using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Dashboard.API.Models;
using GoogleMaps.LocationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        // https://darksky.net/dev/account
        private readonly IWeatherRepository _weatherRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ISettingsRepository _settingsRepo;
        public WeatherController(IWeatherRepository weatherRepo, ISettingsRepository settingsRepo, IMapper mapper, IConfiguration config)
        {
            _settingsRepo = settingsRepo;
            _config = config;
            _mapper = mapper;
            _weatherRepo = weatherRepo;
        }


        [HttpPost]
        [Route("forecast")]
        public async Task<IActionResult> GetForecastForUser(CityForLocationDTO position)
        {
            var clientUsername = User.FindFirst(ClaimTypes.Name).Value;

            // Check cache for forecast for the USER, that's not older then appsettings.WeatherCacheTime
            var forecastCacheBasePath = "./cache/forecasts/";
            var forecastCachePath = forecastCacheBasePath + clientUsername + ".txt";
            if (Directory.Exists(forecastCacheBasePath))
            {
                if (System.IO.File.Exists(forecastCachePath))
                {
                    var cacheContent = System.IO.File.ReadAllText(forecastCachePath);
                    var cachedForecast = JsonConvert.DeserializeObject<ForecastAllForReturnDTO>(cacheContent);
                    var cacheMaxAgeMins = int.Parse(_config.GetSection("AppSettings:WeatherCacheTime").Value);

                    if (cachedForecast.QueryTime.AddMinutes(cacheMaxAgeMins) > DateTime.Now)
                    {
                        // Cache found AND it's recent
                        return Ok(cachedForecast);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(forecastCacheBasePath);
            }
            // Cache not found or old

            // Get which city the user wants the forecast on
            var userSettings = await _settingsRepo.GetSettings(clientUsername);
            string city;
            try
            {
                city = userSettings.Single(x => x.Name == "Overview-Weather").Fields.Single(x => x.Name == "City").Value;
            }
            catch (InvalidOperationException)
            {
                city = "CURRENT_LOCATION";

                _settingsRepo.AddSetting(clientUsername, new Setting(
                    name: "Overview-Weather",
                    fields: new List<SettingField>{
                        new SettingField(
                            name: "City",
                            value: "CURRENT_LOCATION"
                        )
                    }
                ));

                //TODO: _ if the location info is valid, have that as the default instead
            }

            // Get lat-lon data
            string name;
            double latitude;
            double longitude;
            if (city == "CURRENT_LOCATION")
            {
                latitude = position.Latitude;
                longitude = position.Longitude;

                if (latitude == 0 && longitude == 0)
                {
                    /*
                    _settingsRepo.ChangeSettingField(clientUsername, "Overview-Weather", "City", new SettingField(
                        name: "City",
                        value: "Debrecen"
                    ));
                    return BadRequest("Invalid location data! Check if you allowed location information for the website! Changing your city setting to 'Debrecen'.");
                    */
                    name = "Debrecen";
                }
                else
                {

                    string country;
                    name = GetCityFromCords(latitude, longitude, out country);
                    //TODO: _ If not already there, add city to DB
                }
            }
            else
            {
                name = city;
                var cityInfo = await GetCityInfo(name);
                latitude = cityInfo.Latitude;
                longitude = cityInfo.Longitude;
            }

            var forecast = await GetForecast(name, latitude, longitude);

            var forecastString = JsonConvert.SerializeObject(forecast);
            System.IO.File.WriteAllText(forecastCachePath, forecastString);

            return Ok(forecast);
        }


        /// <summary>
        /// Querys the DarkSky API for a forecast on a city
        /// </summary>
        /// <returns></returns>
        private async Task<ForecastAllForReturnDTO> GetForecast(string name, double lat, double lon)
        {
            string urlBase = "https://api.darksky.net/forecast/";
            string apiKey = _config.GetSection("AppSettings:WeatherAPIKey").Value;

            string query = urlBase + apiKey + "/"
                + lat.ToString(CultureInfo.InvariantCulture) + ","
                + lon.ToString(CultureInfo.InvariantCulture)
                + "?units=si";
            JObject weatherInfo;
            using (HttpClient clinet = new HttpClient())
            {
                var httpResponse = await clinet.GetStringAsync(query);
                weatherInfo = JsonConvert.DeserializeObject<JObject>(httpResponse);
            }

            ForecastAllForReturnDTO report = new ForecastAllForReturnDTO();
            report.City = name;
            report.QueryTime = DateTime.Now;

            // Current weather
            report.Currently.InfoTime = report.Currently.InfoTime.AddSeconds(weatherInfo["currently"]["time"].Value<int>()).ToLocalTime();
            report.Currently.Summary = weatherInfo["currently"]["summary"].Value<string>();
            report.Currently.Icon = IconSelect(weatherInfo["currently"]["icon"].Value<string>());
            report.Currently.Temperature = weatherInfo["currently"]["temperature"].Value<float>();
            report.Currently.PercipProbability = weatherInfo["currently"]["precipProbability"].Value<float>();
            report.Currently.PercipIntensity = weatherInfo["currently"]["precipIntensity"].Value<float>();
            report.Currently.TemperatureMax = ForecastDayForReturnDTO.AbsoluteZero;
            report.Currently.TemperatureMin = ForecastDayForReturnDTO.AbsoluteZero;

            // Hourly weather
            for (int i = 0; i < 24; i++)
            {
                var weather = new ForecastDayForReturnDTO();

                weather.InfoTime = weather.InfoTime.AddSeconds(weatherInfo["hourly"]["data"][i]["time"].Value<int>()).ToLocalTime();
                weather.Summary = weatherInfo["hourly"]["data"][i]["summary"].Value<string>();
                weather.Icon = IconSelect(weatherInfo["hourly"]["data"][i]["icon"].Value<string>());
                weather.Temperature = weatherInfo["hourly"]["data"][i]["temperature"].Value<float>();
                weather.PercipProbability = weatherInfo["hourly"]["data"][i]["precipProbability"].Value<float>();
                weather.PercipIntensity = weatherInfo["hourly"]["data"][i]["precipIntensity"].Value<float>();
                weather.TemperatureMax = ForecastDayForReturnDTO.AbsoluteZero;
                weather.TemperatureMin = ForecastDayForReturnDTO.AbsoluteZero;

                report.Hourly.Add(weather);
            }

            // Daily weather
            for (int i = 1; i < 8; i++)
            {
                var weather = new ForecastDayForReturnDTO();

                weather.InfoTime = weather.InfoTime.AddSeconds(weatherInfo["daily"]["data"][i]["time"].Value<int>()).ToLocalTime();
                weather.Summary = weatherInfo["daily"]["data"][i]["summary"].Value<string>();
                weather.Icon = IconSelect(weatherInfo["daily"]["data"][i]["icon"].Value<string>());
                weather.Temperature = ForecastDayForReturnDTO.AbsoluteZero; ;
                weather.PercipProbability = weatherInfo["daily"]["data"][i]["precipProbability"].Value<float>();
                weather.PercipIntensity = weatherInfo["daily"]["data"][i]["precipIntensity"].Value<float>();
                weather.TemperatureMax = weatherInfo["daily"]["data"][i]["temperatureHigh"].Value<float>();
                weather.TemperatureMin = weatherInfo["daily"]["data"][i]["temperatureLow"].Value<float>();

                report.Daily.Add(weather);
            }

            return report;
        }


        /// <summary>
        /// Gets the list of countries stored in the DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("countries")]
        public async Task<IActionResult> GetCountryList()
        {
            var countryList = await _weatherRepo.GetCountries();
            return Ok(countryList);
        }


        /// <summary>
        /// Gets the list of countries stored in the DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("cities/{country}")]
        public async Task<IActionResult> GetCityList(string country)
        {
            var cityList = await _weatherRepo.GetCities(country);
            return Ok(cityList);
        }


        /// <summary>
        /// Querys the GoogleMapsAPI for the city name from latitude and longitude
        /// </summary>
        /// <returns></returns>
        private string GetCityFromCords(double lat, double lon, out string country)
        {
            var geoCodingApiKey = _config.GetSection("AppSettings:GeoCodingAPIKey").Value;
            var locationService = new GoogleLocationService(geoCodingApiKey);
            var address = locationService.GetAddressFromLatLang(lat, lon);
            country = address.Country;
            return address.City;
        }


        /// <summary>
        /// Gets city coordinates (lat, lon) from the DB
        /// </summary>
        /// <returns></returns>
        private async Task<CityForLocationDTO> GetCityInfo(string city)
        {
            var cityInfo = await _weatherRepo.GetCityInfo(city);
            var cityInfoToReturn = _mapper.Map<CityForLocationDTO>(cityInfo);
            return cityInfoToReturn;
        }


        /// <summary>
        /// A lookup table between the DarkSkyAPI and FontAwesome weather icons
        /// </summary>
        /// <returns></returns>
        private string IconSelect(string apiIcon)
        {
            string fontAwesomeIcon;
            switch (apiIcon)
            {
                case "clear-day":
                    fontAwesomeIcon = "sun";
                    break;
                case "clear-night":
                    fontAwesomeIcon = "moon";
                    break;
                case "rain":
                    fontAwesomeIcon = "cloud-showers-heavy";
                    break;
                case "snow":
                    fontAwesomeIcon = "snowflake";
                    break;
                case "sleet":
                    fontAwesomeIcon = "snowflake";
                    break;
                case "wind":
                    fontAwesomeIcon = "wind";
                    break;
                case "fog":
                    fontAwesomeIcon = "smog";
                    break;
                case "cloudy":
                    fontAwesomeIcon = "cloud";
                    break;
                case "partly-cloudy-day":
                    fontAwesomeIcon = "cloud-sun";
                    break;
                case "partly-cloudy-night":
                    fontAwesomeIcon = "cloud-moon";
                    break;
                default:
                    fontAwesomeIcon = "umbrella";
                    break;
            }

            return fontAwesomeIcon;
        }
    }
}