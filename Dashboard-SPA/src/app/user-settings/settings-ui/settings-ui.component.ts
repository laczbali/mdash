import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { SettingsData } from 'src/app/_models/SettingsData';
import { interval } from 'rxjs';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SettingsService } from 'src/app/_services/settings.service';

@Component({
  selector: 'app-settings-ui',
  templateUrl: './settings-ui.component.html',
  styleUrls: ['./settings-ui.component.css']
})

// TODO: ! If there is a custom color set, but not saved, keep it selected when the user navigates back to the menu
// TODO: _ show recently used background images
// TODO: _ have links for unsplash, reddit, etc

export class SettingsUiComponent implements OnInit {
  colorSettings: SettingsData;
  selectedUIColor = new FormControl();
  uiColor = 'rgb(255,255,255)';
  selectedTextColor = new FormControl();
  textColor = 'rgb(255,255,255)';
  selectedBackgroundColor = new FormControl();
  backgroundColor = 'rgb(255,255,255)';

  backgroundImage = new FormControl();
  backgroundGradient: number;

  constructor(private http: HttpClient, private alertify: AlertifyService, private settingsService: SettingsService) { }

  ngOnInit() {
    this.initDefaults();
    this.getColorSettings();
    this.getBackgroundSettings();
  }

  initDefaults() {
    const defaultUi0 = this.buildRgbString(this.settingsService.defaultColors.defaultUiColor0);
    (<HTMLElement>document.querySelector('#defaultUiColor0')).style.backgroundColor = defaultUi0;

    const defaultUi1 = this.buildRgbString(this.settingsService.defaultColors.defaultUiColor1);
    (<HTMLElement>document.querySelector('#defaultUiColor1')).style.backgroundColor = defaultUi1;

    const defaultText0 = this.buildRgbString(this.settingsService.defaultColors.defaultTextColor0);
    (<HTMLElement>document.querySelector('#defaultTextColor0')).style.backgroundColor = defaultText0;

    const defaultBack0 = this.buildRgbString(this.settingsService.defaultColors.defaultBackgroundColor0);
    (<HTMLElement>document.querySelector('#defaultBackgroundColor0')).style.backgroundColor = defaultBack0;
  }

  /*************************************************************************************************************************
      Buttons
  *************************************************************************************************************************/
  saveButtonClicked() {
    this.saveColorSettings();
    this.saveBackgroundSettings();
    this.alertify.success('Saved!');
  }
  cancelButtonClicked() {
    this.getColorSettings();
    this.getBackgroundSettings();
    this.alertify.warning('Settings reset!');
  }

  /*************************************************************************************************************************
      Background
  *************************************************************************************************************************/
  getBackgroundSettings() {
    this.http.get(environment.apiUrl + 'settings/get/background').subscribe(
      next => {
        const backgroundSettings = <SettingsData>next;
        backgroundSettings.fields.forEach(field => {
          switch (field.name) {
            case 'ImageURL':
              this.backgroundImage.setValue(field.value);
              this.backgroundImageClicked();
              break;

            case 'Gradient':
              this.backgroundGradient = parseFloat(field.value) * 100;
              this.backgroundGradientChanged();
              break;
            default: break;
          }
        });
      }
    );
  }

  saveBackgroundSettings() {
    const backgroundSettings: SettingsData = {
      name: 'General-Background',
      fields: [
        {
          name: 'ImageURL',
          value: this.backgroundImage.value,
          type: 'url'
        },
        {
          name: 'Gradient',
          value: String(this.backgroundGradient / 100),
          type: 'float'
        }
      ]
    };

    this.http.post(environment.apiUrl + 'settings/set', backgroundSettings).subscribe(next => { });
  }

  backgroundImageClicked() {
    const urlString = 'url(' + this.backgroundImage.value + ')';
    document.body.style.setProperty('--background-image-url', urlString);
  }

  backgroundGradientChanged() {
    // The backgroundGradient doesen't upadte it's value right away for some reason. This is the fix.
    const gradientUpdate = interval(100).subscribe(
      next => {
        const gradientValue = this.backgroundGradient / 100;
        document.body.style.setProperty('--backgrond-gradient', gradientValue.toString());

        gradientUpdate.unsubscribe();
      }
    );
  }

  /*************************************************************************************************************************
      COLOR
  *************************************************************************************************************************/
  saveColorSettings() {
    const colorSettings: SettingsData = {
      name: 'General-Colors',
      fields: [
        {
          name: 'UIBase',
          value: (this.selectedUIColor.value === 'CUSTOM') ?
            getComputedStyle(document.body).getPropertyValue('--main-color') :
            String(this.selectedUIColor.value).split('_')[1],
          type: (this.selectedUIColor.value === 'CUSTOM') ? 'CUSTOM' : 'DEFAULT'
        },
        {
          name: 'TextBase',
          value: (this.selectedTextColor.value === 'CUSTOM') ?
            getComputedStyle(document.body).getPropertyValue('--text-color') :
            String(this.selectedTextColor.value).split('_')[1],
          type: (this.selectedTextColor.value === 'CUSTOM') ? 'CUSTOM' : 'DEFAULT'
        },
        {
          name: 'BackgroundBase',
          value: (this.selectedBackgroundColor.value === 'CUSTOM') ?
            getComputedStyle(document.body).getPropertyValue('--background-color') :
            String(this.selectedBackgroundColor.value).split('_')[1],
          type: (this.selectedBackgroundColor.value === 'CUSTOM') ? 'CUSTOM' : 'DEFAULT'
        }
      ]
    };

    this.http.post(environment.apiUrl + 'settings/set', colorSettings).subscribe(next => { });
  }

  getColorSettings() {
    this.http.get(environment.apiUrl + 'settings/get/colors').subscribe(
      next => {
        this.colorSettings = <SettingsData>next;
        this.fillColorSettings();
      }
    );
  }

  fillColorSettings() {
    this.colorSettings.fields.forEach(field => {
      switch (field.name) {
        case 'UIBase':
          if (field.type === 'DEFAULT') {
            this.selectedUIColor.setValue('DEFAULT_' + field.value);
          } else {
            this.selectedUIColor.setValue('CUSTOM');
            this.uiColor = this.buildRgbString(field.value);
          }
          break;

        case 'TextBase':
          if (field.type === 'DEFAULT') {
            this.selectedTextColor.setValue('DEFAULT_' + field.value);
          } else {
            this.selectedTextColor.setValue('CUSTOM');
            this.textColor = this.buildRgbString(field.value);
          }
          break;

        case 'BackgroundBase':
          if (field.type === 'DEFAULT') {
            this.selectedBackgroundColor.setValue('DEFAULT_' + field.value);
          } else {
            this.selectedBackgroundColor.setValue('CUSTOM');
            this.backgroundColor = this.buildRgbString(field.value);
          }
          break;

        default: break;
      }

      this.uiColorSelectionChanged();
      this.textColorSelectionChanged();
      this.backgroundColorSelectionChanged();
    });
  }

  uiColorSelectionChanged() {
    let defaultColor: string;
    let rgbString: string;

    switch (this.selectedUIColor.value) {
      case 'CUSTOM':
        this.uiColorChanged();
        break;
      case 'DEFAULT_0':
        defaultColor = window.getComputedStyle(document.querySelector('#defaultUiColor0')).backgroundColor;
        rgbString = this.stripRgbString(defaultColor);
        document.body.style.setProperty('--main-color', rgbString);
        break;
      case 'DEFAULT_1':
        defaultColor = window.getComputedStyle(document.querySelector('#defaultUiColor1')).backgroundColor;
        rgbString = this.stripRgbString(defaultColor);
        document.body.style.setProperty('--main-color', rgbString);
        break;
      default: break;
    }
  }

  textColorSelectionChanged() {
    let defaultColor: string;
    let rgbString: string;

    switch (this.selectedTextColor.value) {
      case 'CUSTOM':
        this.textColorChanged();
        break;
      case 'DEFAULT_0':
        defaultColor = window.getComputedStyle(document.querySelector('#defaultTextColor0')).backgroundColor;
        rgbString = this.stripRgbString(defaultColor);
        document.body.style.setProperty('--text-color', rgbString);
        break;
      default: break;
    }
  }

  backgroundColorSelectionChanged() {
    let defaultColor: string;
    let rgbString: string;

    switch (this.selectedBackgroundColor.value) {
      case 'CUSTOM':
        this.backgroundColorChanged();
        break;
      case 'DEFAULT_0':
        defaultColor = window.getComputedStyle(document.querySelector('#defaultBackgroundColor0')).backgroundColor;
        rgbString = this.stripRgbString(defaultColor);
        document.body.style.setProperty('--background-color', rgbString);
        break;
      default: break;
    }
  }

  uiColorChanged() {
    this.selectedUIColor.setValue('CUSTOM');
    const rgbString = this.stripRgbString(this.uiColor);
    document.body.style.setProperty('--main-color', rgbString);
  }

  textColorChanged() {
    this.selectedTextColor.setValue('CUSTOM');
    const rgbString = this.stripRgbString(this.textColor);
    document.body.style.setProperty('--text-color', rgbString);
  }

  backgroundColorChanged() {
    this.selectedBackgroundColor.setValue('CUSTOM');
    const rgbString = this.stripRgbString(this.backgroundColor);
    document.body.style.setProperty('--background-color', rgbString);
  }

  stripRgbString(rgbString: string) {
    rgbString = rgbString.substring(4); // strip front 'rgb('
    rgbString = rgbString.substring(0, rgbString.length - 1); // strip back ')'
    rgbString = rgbString.replace(/,/g, ', '); // Add spaces
    return rgbString;
  }

  buildRgbString(rgbString: string) {
    rgbString = 'rgb(' + rgbString + ')';
    return rgbString;
  }
}
