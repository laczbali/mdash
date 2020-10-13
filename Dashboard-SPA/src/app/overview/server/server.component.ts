import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';
import { Vpn } from 'src/app/_models/Vpn';
import { Drive } from 'src/app/_models/Drive';
import { interval } from 'rxjs';
import { ServerStatus } from 'src/app/_models/ServerStatus';
import { Process } from 'src/app/_models/Process';
import { Resources } from 'src/app/_models/Resources';

@Component({
  selector: 'app-server',
  templateUrl: './server.component.html',
  styleUrls: ['./server.component.css']
})
export class ServerComponent implements OnInit {
  // gauge: https://github.com/ashish-chopra/ngx-gauge

  constructor(private http: HttpClient, private alertify: AlertifyService) { }

  vpnData: Vpn = {
    updatedTime: null,
    clientList: [],
    sumTrafficMbytes: 0
  };

  storageData: Drive[] = [];

  systemResources: Resources = {
    memoryUsedGb: 0,
    memoryAllGb: 0,
    processorUsedPercent: 0
  };

  processes: Process[] = [];

  gaugeForeground = '';
  gaugeBackground = '';

  ngOnInit() {
    const textColor = getComputedStyle(document.body).getPropertyValue('--text-color');
    const backgroundColor = getComputedStyle(document.body).getPropertyValue('--background-color');
    this.gaugeForeground = this.generateRgbaString(textColor, '1');
    this.gaugeBackground = this.generateRgbaString(backgroundColor, '1');

    this.getData(false);

    const updateTick = interval(1010 * 60 * 30); // ~15 minutes
    updateTick.subscribe(
      next => {
        this.getData(false);
      }
    );

  }

  getData(force: boolean) {
    if (force === false) {
      const storedDataString = localStorage.getItem('server');
      if (storedDataString !== null) {
        const jsonObject = JSON.parse(storedDataString);
        const storedData = <ServerStatus>jsonObject;
        const storedTime = new Date(storedData.querytime);
        const deltaMilis = 1000 * 60 * 15; // 15 minutes

        if (storedTime.getTime() + deltaMilis > Date.now()) {
          // Use cache data
          this.parseData(storedData);
          return;
        }
      }
    }

    this.http.get(environment.apiUrl + 'system/status').subscribe(
      next => {
        const serverData = <ServerStatus>next;
        serverData.querytime = Date.now();

        const jsonString = JSON.stringify(serverData);
        localStorage.setItem('server', jsonString);

        if (serverData.errors !== null) { console.log('Server stat errors:', serverData.errors); }

        this.parseData(serverData);
        return;
      },
      error => {
        console.log(error);
        return;
      }
    );
  }

  parseData(serverData: ServerStatus) {
    if(serverData.vpn !== null) { this.vpnData = serverData.vpn; }
    if(serverData.drives !== null) { this.storageData = serverData.drives; }
    if(serverData.processes !== null) { this.processes = serverData.processes; }
    if(serverData.resources !== null) { this.systemResources = serverData.resources; }
  }

  getQueryTime() {
    const storedDataString = localStorage.getItem('server');
    if(storedDataString === null || storedDataString === '') {
      return 'N/A';
    }

    const jsonObject = JSON.parse(storedDataString);
    const storedData = <ServerStatus>jsonObject;
    const querytime = storedData.querytime;

    if (querytime === null || querytime.valueOf() === 0) {
      return 'N/A';
    }

    const date = new Date(querytime);
    return date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0');
  }

  generateRgbaString(rgbString: string, alpha: string) {
    return 'rgba(' + rgbString + ', ' + alpha + ')';
  }

}
