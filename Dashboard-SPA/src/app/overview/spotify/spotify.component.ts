import { Component, OnInit } from '@angular/core';
import { HelperService } from 'src/app/_services/helper.service';

@Component({
  selector: 'app-spotify',
  templateUrl: './spotify.component.html',
  styleUrls: ['./spotify.component.css']
})
export class SpotifyComponent implements OnInit {
  /* URLS:
   * https://developer.spotify.com/dashboard/applications
   * https://developer.spotify.com/documentation/web-api/quick-start/
   * https://developer.spotify.com/documentation/web-playback-sdk/quick-start/
   * https://developer.spotify.com/documentation/general/guides/authorization-guide/#authorization-code-flow
  */

  constructor(private helper: HelperService) { }

  ngOnInit() {
  }

  login() {
    const spAuthBase = 'https://accounts.spotify.com/authorize';
    let spAuthUrl = spAuthBase + '?';
    spAuthUrl += 'client_id=fc7dcb5fb1de420eb27dfb2d9228d3fd';
    spAuthUrl += '&response_type=code';
    spAuthUrl += '&redirect_uri=' + this.helper.encodeString(window.location.href);
    spAuthUrl += '&scope=';

    console.log(spAuthUrl);
  }

}
