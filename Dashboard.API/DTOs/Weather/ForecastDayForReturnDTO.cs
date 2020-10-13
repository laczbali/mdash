using System;

namespace Dashboard.API.DTOs
{
    public class ForecastDayForReturnDTO
    {
        public ForecastDayForReturnDTO()
        {
            InfoTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
        }
        public DateTime InfoTime { get; set; }
        public string Summary { get; set; }
        public string Icon { get; set; }
        public float Temperature { get; set; }
        public float TemperatureMax { get; set; }
        public float TemperatureMin { get; set; }
        public float PercipProbability { get; set; }
        public float PercipIntensity { get; set; }
        public static float AbsoluteZero = (float)-273.15;
    }
}