using System;
using System.Globalization;
using Dashboard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        public async void SeedDB()
        {
            if (!await _context.Cities.AnyAsync())
            {
                // SeedCities();
            }

            if (!await _context.Users.AnyAsync())
            {
                SeedAdmin();
            }
        }

        private void SeedCities()
        {
            var filePath = "./Data/SeedData/world-cities.csv";
            if (!System.IO.File.Exists(filePath))
            {
                filePath = "./world-cities.csv";
            }

            var cityList = System.IO.File.ReadAllLines(filePath);

            // Skip header line
            for (int i = 1; i < cityList.Length; i++)
            {
                var entry = cityList[i].Split(',');
                var entryName = entry[0].Substring(1, entry[0].Length - 2);
                var entryLat = float.Parse(entry[2].Substring(1, entry[2].Length - 2), CultureInfo.InvariantCulture);
                var entryLong = float.Parse(entry[3].Substring(1, entry[3].Length - 2), CultureInfo.InvariantCulture);
                var entryCountry = entry[4].Substring(1, entry[4].Length - 2);

                var city = new City
                {
                    Name = entryName,
                    Latitude = entryLat,
                    Longitude = entryLong,
                    Country = entryCountry
                };

                _context.Cities.Add(city);
            }

            _context.SaveChanges();
        }

        private void SeedAdmin()
        {

        }
    }
}