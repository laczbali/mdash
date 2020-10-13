import { Component, OnInit, HostListener } from '@angular/core';
import { FormControl } from '@angular/forms';
import { SettingsData } from 'src/app/_models/SettingsData';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-settings-overview',
  templateUrl: './settings-overview.component.html',
  styleUrls: ['./settings-overview.component.css']
})
export class SettingsOverviewComponent implements OnInit {
  // Screen info
  screenHeight: any;
  screenWidth: any;

  // Form controls
  torrentUrl = new FormControl();
  torrentUser = new FormControl();
  torrentPass = new FormControl();

  constructor(private http: HttpClient, private alertify: AlertifyService) { }

  ngOnInit() {
    this.onResize();

    this.getTorrentSettings();
  }

  saveButtonClicked() {
    this.saveTorrentSettings();
  }

  cancelButtonClicked() {
    this.getTorrentSettings();
  }

  getTorrentSettings() {
    this.http.get(environment.apiUrl + 'settings/get/torrentapi').subscribe(
      next => {
        const torrentSetttings = <SettingsData>next;
        torrentSetttings.fields.forEach(field => {
          switch (field.name) {
            case 'url':
              this.torrentUrl.setValue(field.value);
              break;
            case 'username':
              this.torrentUser.setValue(field.value);
              break;
            default: break;
          }
        });
      }
    );
    this.torrentPass.setValue('');
  }

  saveTorrentSettings() {
    const torrentSetttings: SettingsData = {
      name: 'Overview-Torrent',
      fields: [
        {
          name: 'url',
          value: this.torrentUrl.value,
          type: 'url'
        },
        {
          name: 'username',
          value: this.torrentUser.value,
          type: ''
        },
        {
          name: 'password',
          value: this.torrentPass.value,
          type: 'password'
        }
      ]
    };

    this.http.post(environment.apiUrl + 'settings/set', torrentSetttings).subscribe(next => {
      this.alertify.success('Saved qBitTorrent settings');
    });
    this.torrentPass.setValue('');
  }

  // Help displaying
  @HostListener('window:resize', ['$event'])
  onResize(event?) {
    this.screenHeight = window.innerHeight;
    this.screenWidth = window.innerWidth;
  }

  // TODO: _ implement features:
  // Set _ which cards should appear
  // Set _ card positions
}
