import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { SettingsData } from '../_models/SettingsData';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  loadCount = 0;

  constructor(private http: HttpClient) { }

  defaultColors = {
    defaultUiColor0: '0, 151, 230',
    defaultUiColor1: '149, 149, 149',
    defaultTextColor0: '236, 240, 241',
    defaultBackgroundColor0: '34, 34, 34'
  };

  loadUI() {
    this.getBackgroundSettings();
    this.getColorSettings();
  }

  loaded() {
    if (this.loadCount === 2) {
      return true;
    }
    return false;
  }

  private getColorSettings() {
    this.http.get(environment.apiUrl + 'settings/get/colors').subscribe(
      next => {
        const colorSettings = <SettingsData>next;
        colorSettings.fields.forEach(field => {
          switch (field.name) {
            case 'UIBase':
              let uiColor: string;
              if (field.type === 'DEFAULT') {
                uiColor = this.defaultColors['defaultUiColor' + field.value];
              } else {
                uiColor = field.value;
              }
              document.body.style.setProperty('--main-color', uiColor);
              break;

            case 'TextBase':
              let textColor: string;
              if (field.type === 'DEFAULT') {
                textColor = this.defaultColors['defaultTextColor' + field.value];
              } else {
                textColor = field.value;
              }
              document.body.style.setProperty('--text-color', textColor);
              break;

            case 'BackgroundBase':
              let backColor: string;
              if (field.type === 'DEFAULT') {
                backColor = this.defaultColors['defaultBackgroundColor' + field.value];
              } else {
                backColor = field.value;
              }
              document.body.style.setProperty('--background-color', backColor);
              break;

            default: break;
          }
        });

        this.loadCount++;
      },
      () => {
        this.loadCount++;
      }
    );
  }

  private getBackgroundSettings() {
    this.http.get(environment.apiUrl + 'settings/get/background').subscribe(
      next => {
        const backgroundSettings = <SettingsData>next;
        backgroundSettings.fields.forEach(field => {
          switch (field.name) {
            case 'ImageURL':
              const urlString = 'url(' + field.value + ')';
              document.body.style.setProperty('--background-image-url', urlString);
              break;

            case 'Gradient':
              document.body.style.setProperty('--backgrond-gradient', field.value);
              break;

            default: break;
          }
        });

        this.loadCount++;
      },
      () => {
        this.loadCount++;
      }
    );
  }
}
