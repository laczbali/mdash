import { WeatherDataDay } from './WeatherDataDay';

export interface WeatherData {
    city: string;
    queryTime: string;
    currently: WeatherDataDay;
    hourly: WeatherDataDay[];
    daily: WeatherDataDay[];
}
