import { Component, OnInit, HostListener } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { TorrentData } from 'src/app/_models/TorrentData';
import { interval } from 'rxjs';

@Component({
  selector: 'app-qbittorrent',
  templateUrl: './qbittorrent.component.html',
  styleUrls: ['./qbittorrent.component.css']
})

// TODO: ^ Keep counting down the time-remaining indicator between updates
// TODO: ^ Have the updated@ indicator update manually
// TODO: _ Have the qBittorrent link open the webui of the user

export class QbittorrentComponent implements OnInit {
  // Screen info
  screenHeight: any;
  screenWidth: any;

  downloadData: TorrentData = {
    downloads: [],
    numOfUpload: 0,
    totalDlSpeed: 0,
    totalTorrents: 0,
    totalUpSpeed: 0,
    querytime: new Date(0),
    maxEta: 0,
    uiUrl: ''
  };

  constructor(private http: HttpClient, private alertify: AlertifyService) { }

  // Help displaying
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
    this.screenHeight = window.innerHeight;
    this.screenWidth = window.innerWidth;
  }

  ngOnInit() {
    this.onResize();

    this.getTorrentData(false);
    // TODO: ^ get polling interval from user settings (with reasonable limits)
    const tick = interval(1100 * 60 * 3); // 3.3 minutes
    tick.subscribe(
      next => {
        this.getTorrentData(false);
      }
    );
  }

  getTorrentData(force: boolean) {
    // TODO: ^ cache handling could be a service
    if (force === false) {
      const storedDataString = localStorage.getItem('torrent');
      if (storedDataString !== null) {
        const jsonObject = JSON.parse(storedDataString);
        const storedData = <TorrentData>jsonObject;
        const storedTime = new Date(storedData.querytime);
        const deltaMilis = 1000 * 60 * 3; // 3 minutes

        if (storedTime.getTime() + deltaMilis > Date.now()) {
          // Use cache data
          this.downloadData = storedData;
          return;
        }
      }
    }

    this.http.get(environment.apiUrl + 'torrent/list').subscribe(
      next => {
        this.downloadData = <TorrentData>next;
        const jsonString = JSON.stringify(next);
        localStorage.setItem('torrent', jsonString);
      },
      error => {
        // If 400, display alertify popup
        if (error.status === 400) {
          this.alertify.warning('qBittorrent API not configured. <a href="settings/overview">Configure now?</a>');
        } else {
          this.alertify.error('Error getting qBittorrent info');
        }
      }
    );
  }

  getItemName(name: string) {
    // Strip string according to screen size, append '...' if neccessary
    let newName: string;

    if (this.screenWidth >= 1620) {
      newName = name.substr(0, 38);
    } else {
      newName = name.substr(0, 32);
    }

    if (newName.length !== name.length) {
      newName += '...';
    }

    return newName;
  }

  getEta(eta: number) {
    const hours = Math.floor(eta / 3600);
    const minutes = Math.floor(eta % 3600 / 60);

    if (hours > 24) { return 'N/A'; }

    return hours.toString() + 'h ' + minutes.toString() + 'm';
  }

  getQueryTime() {
    if (this.downloadData.querytime.valueOf() === 0) {
      return 'N/A';
    }

    const date = new Date(this.downloadData.querytime);
    return date.getHours().toString() + ':' + date.getMinutes().toString().padStart(2, '0');
  }

  // Note:
  // Original plan was to make the API request from the client site,
  // but that can't be done (at least at the moment) due to the
  // qBittorent WebAPI CORS policy.
}
