using System;
using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class ForecastAllForReturnDTO
    {
        public DateTime QueryTime { get; set; }
        public string City { get; set; }
        public ForecastAllForReturnDTO()
        {
            Currently = new ForecastDayForReturnDTO();
            Hourly = new List<ForecastDayForReturnDTO>();
            Daily = new List<ForecastDayForReturnDTO>();
        }
        public ForecastDayForReturnDTO Currently { get; set; }
        public ICollection<ForecastDayForReturnDTO> Hourly { get; set; }
        public ICollection<ForecastDayForReturnDTO> Daily { get; set; }
    }
}