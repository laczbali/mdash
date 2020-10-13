using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.API.Models;

namespace Dashboard.API.Data
{
    public interface IWeatherRepository
    {
        Task<List<string>> GetCountries();
        Task<List<string>> GetCities(string country);
        Task<City> GetCityInfo(string cityName);
    }
}