import { Component, OnInit, HostListener } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { ColorspacesService } from '../_services/colorspaces.service';

@Component({
    selector: 'app-nav',
    templateUrl: './nav.component.html',
    styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
    // TODO: _ switch logo to white, under HSV V<50%, and to black at V>50%

    // Screen info
    screenHeight: any;
    screenWidth: any;

    constructor(
        public authService: AuthService,
        private alertify: AlertifyService,
        private router: Router,
        private colorspace: ColorspacesService
    ) { }

    ngOnInit() {
        if (this.loggedIn()) {
            // this.router.navigate(['/overview']);
        }

        this.onResize();
    }

    loggedIn() {
        return this.authService.loggedIn();
    }

    logout() {
        localStorage.removeItem('token');
        this.alertify.message('Logged out');
        this.router.navigate(['/home']);
    }

    // Help displaying
    @HostListener('window:resize', ['$event'])
    onResize(event?) {
        this.screenHeight = window.innerHeight;
        this.screenWidth = window.innerWidth;
    }

    isTextDark() {
        const textColor = document.body.style.getPropertyValue('--text-color');
        const rgbValues = this.colorspace.parseRgbString(textColor);
        const hslValues = this.colorspace.rgbToHsl(rgbValues[0], rgbValues[1], rgbValues[2]);
        if (hslValues[2] < 0.5) {
            return true;
        }
        return false;
    }
}
