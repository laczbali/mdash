import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HelperService {

  constructor() { }

  /**
   * Replaces ':' with '%3A', and '/' with '%2F'
   * @param input
   */
  encodeString(input: string) {
    input = input.replace(/:/g, '%3A');
    input = input.replace(/\//g, '%2F');
    return input;
  }

}
