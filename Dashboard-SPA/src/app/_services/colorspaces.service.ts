import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ColorspacesService {

  constructor() { }

  /**
   * Converts '0, 1, 2' to [0, 1, 2]
   */
  parseRgbString(rgbString: string) {
    const stringArray = rgbString.split(', ');
    return [
      parseInt(stringArray[0], 10),
      parseInt(stringArray[1], 10),
      parseInt(stringArray[2], 10)
    ];
  }

  rgbToHsl(r, g, b) {
    r /= 255, g /= 255, b /= 255;

    const max = Math.max(r, g, b), min = Math.min(r, g, b);
    let h, s;
    const l = (max + min) / 2;

    if (max === min) {
      h = s = 0; // achromatic
    } else {
      const d = max - min;
      s = l > 0.5 ? d / (2 - max - min) : d / (max + min);

      switch (max) {
        case r: h = (g - b) / d + (g < b ? 6 : 0); break;
        case g: h = (b - r) / d + 2; break;
        case b: h = (r - g) / d + 4; break;
      }

      h /= 6;
    }

    return [h, s, l];
  }
}
