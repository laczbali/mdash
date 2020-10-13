using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly DataContext _context;
        public WeatherRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<City> GetCityInfo(string cityName)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name.ToLower() == cityName.ToLower());
            return city;
        }

        public async Task<List<string>> GetCities(string country)
        {
            var cities = await _context.Cities.Where(x => x.Country.ToLower() == country.ToLower()).Select(x => x.Name).ToListAsync();
            cities.Sort();
            return cities;
        }

        public async Task<List<string>> GetCountries()
        {
            var countries = await _context.Cities.Select(x => x.Country).Distinct().ToListAsync();
            countries.Sort();
            return countries;
        }


    }
}