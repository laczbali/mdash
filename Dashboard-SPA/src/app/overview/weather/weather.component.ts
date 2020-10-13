import { Component, OnInit, ViewChild } from '@angular/core';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { Label, Color, BaseChartDirective } from 'ng2-charts';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { WeatherData } from 'src/app/_models/WeatherData';
import { interval } from 'rxjs';
import { WeatherDataDay } from 'src/app/_models/WeatherDataDay';
import { HostListener } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-weather',
  templateUrl: './weather.component.html',
  styleUrls: ['./weather.component.css']
})
export class WeatherComponent implements OnInit {
  // Screen info
  screenHeight: any;
  screenWidth: any;

  // Top row data
  iconMain = 'cloud';
  tempMain = '0';
  summaryMain = '';

  // Mid row data
  lineChartData: ChartDataSets[] = [
    { data: [], label: 'Temperature' }
  ];
  lineChartLabels: Label[] = [];

  // Bottom row data
  dailyWeather: WeatherDataDay[] = [];

  // Footer data
  positionDto: any = {};
  sourceCity = 'Debrecen';
  queryTime = '00:00';

  // Chart options
  @ViewChild(BaseChartDirective, { static: true }) chart: BaseChartDirective; // Allows: this.chart.update()
  lineChartOptions: (ChartOptions & { annotation: any }) = {
    responsive: true,
    scales: {
      xAxes: [{
        gridLines: { color: 'rgba(255,255,255,0.1)' },
        ticks: { fontColor: 'rgba(255,255,255,0.3)' }
      }],
      yAxes: [
        {
          id: 'y-axis-0',
          position: 'left',
          gridLines: { color: 'rgba(255,255,255,0.1)' },
          ticks: { fontColor: 'rgba(255,255,255,0.3)' }
        }
      ]
    },
    annotation: {},
  };
  lineChartColors: Color[] = [
    {
      borderColor: 'rgba(148,159,177,1)', // Line
      backgroundColor: 'rgba(148,159,177,0.2)', // Area under the line
      pointBackgroundColor: 'rgba(148,159,177,1)', // Point
      pointBorderColor: '#fff', // Point border
    }
  ];
  lineChartType = 'line';

  // Class basics
  constructor(private http: HttpClient, private alertify: AlertifyService) { }
  ngOnInit() {
    this.onResize();

    this.getLocation();
    this.getWeather();
    const tick = interval(1100 * 60 * 5); // 5.5 minutes
    tick.subscribe(
      next => {
        this.getWeather();
      }
    );

    // document.body.style.setProperty('--main-color', '200, 200, 200');
    const textColor = getComputedStyle(document.body).getPropertyValue('--text-color');
    this.lineChartColors[0].borderColor = this.generateRgbaString(textColor, '0.6'); // Line
    this.lineChartColors[0].backgroundColor = this.generateRgbaString(textColor, '0.2'); // Area under the line
    this.lineChartColors[0].pointBackgroundColor = this.generateRgbaString(textColor, '0.5'); // Point
    this.lineChartColors[0].pointBorderColor = this.generateRgbaString(textColor, '1'); // Point border

    const backgroundColor = getComputedStyle(document.body).getPropertyValue('--background-color');
    this.lineChartOptions.scales.xAxes[0].gridLines.color = this.generateRgbaString(backgroundColor, '0.2');
    this.lineChartOptions.scales.xAxes[0].ticks.fontColor = this.generateRgbaString(backgroundColor, '0.6');
    this.lineChartOptions.scales.yAxes[0].gridLines.color = this.generateRgbaString(backgroundColor, '0.2');
    this.lineChartOptions.scales.yAxes[0].ticks.fontColor = this.generateRgbaString(backgroundColor, '0.6');
  }

  // Help displaying
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
    this.screenHeight = window.innerHeight;
    this.screenWidth = window.innerWidth;
  }

  // TODO: ^ pretty sure this does not work well
  // Try to get the location
  getLocation() {
    navigator.geolocation.getCurrentPosition(
      next => {
        if (next) {
          const coords = {
            lat: next.coords.latitude,
            lon: next.coords.longitude,
            time: next.timestamp
          };
          localStorage.setItem('position', JSON.stringify(coords));
        }
      },
      error => {
        console.log('Geolocation error');
        const coords = {
          lat: 0,
          lon: 0,
          time: 0
        };
        localStorage.setItem('position', JSON.stringify(coords));
      }
    );
  }

  // Get and Parse weather
  async getWeather() {
    let positionString = localStorage.getItem('position');
    let tries = 0;
    while (positionString === null && tries < 150) {
      // If we can't find it first, give it some time
      await this.delay(10);
      positionString = localStorage.getItem('position');
      tries++;
    }

    if (!(positionString === null)) {
      const position = JSON.parse(positionString);
      this.positionDto.Latitude = position.lat;
      this.positionDto.Longitude = position.lon;
    } else {
      this.positionDto.Latitude = 0;
      this.positionDto.Longitude = 0;
    }

    let haveStoredWeather = false;
    const storedWeather = localStorage.getItem('weather');
    if (!(storedWeather === null)) {
      haveStoredWeather = true;
      const jsonObject = JSON.parse(storedWeather);
      const weather = <WeatherData>jsonObject;
      const storedTime = new Date(weather.queryTime);
      const deltaMilis = 1000 * 60 * 5; // 5 minutes

      if (storedTime.getTime() + deltaMilis > Date.now()) {
        this.parseWeather(storedWeather);
        return;
      }
    }

    // const options = { headers: new HttpHeaders().set('Content-Type', 'application/json') };
    this.http.post(environment.apiUrl + 'weather/forecast', this.positionDto).subscribe(
      next => {
        const jsonString = JSON.stringify(next);
        localStorage.setItem('weather', jsonString);
        this.parseWeather(jsonString);
      },
      error => {
        console.log(error);
        this.alertify.error('Error updating the weather!');
        if (haveStoredWeather) {
          this.parseWeather(storedWeather);
        }
      }
    );

    // Update our location
    this.getLocation();
  }

  parseWeather(jsonString) {
    const jsonObject = JSON.parse(jsonString);
    const weather = <WeatherData>jsonObject;

    this.iconMain = weather.currently.icon;
    this.tempMain = weather.currently.temperature.toFixed(0);
    this.summaryMain = weather.currently.summary;
    const queryDate = new Date(weather.queryTime);
    const queryDateHours = queryDate.getHours().toString();
    let queryDateMinutes = queryDate.getMinutes().toString();
    if (queryDateMinutes.length < 2) {
      queryDateMinutes = '0' + queryDateMinutes;
    }
    this.queryTime = queryDateHours + ':' + queryDateMinutes;
    this.sourceCity = weather.city;

    for (let i = 0; i < 24; i++) {
      this.lineChartData[0].data[i] = weather.hourly[i].temperature;
      this.lineChartLabels[i] = new Date(weather.hourly[i].infoTime).getHours().toString() + ':00';
    }
    this.chart.update();

    for (let i = 0; i < 7; i++) {
      const day: WeatherDataDay = {
        infoTime: this.dayLookup(new Date(weather.daily[i].infoTime).getDay()),
        icon: weather.daily[i].icon,
        temperatureMax: Number.parseInt(weather.daily[i].temperatureMax.toFixed(0), 10),
        temperatureMin: Number.parseInt(weather.daily[i].temperatureMin.toFixed(0), 10)
      };
      this.dailyWeather[i] = day;
    }
  }

  dayLookup(dayNumber) {
    const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    return days[dayNumber];
  }

  generateRgbaString(rgbString: string, alpha: string) {
    return 'rgba(' + rgbString + ', ' + alpha + ')';
  }

  async delay(ms: number) {
    await new Promise(resolve => setTimeout(() => resolve(), ms)).then();
  }
}
