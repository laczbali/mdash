import { Component, OnInit, HostListener } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { SettingsService } from './_services/settings.service';
import { trigger, transition, query, style, group, animate } from '@angular/animations';

// TODO: _ Work on animations:
@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    animations: [
        trigger('routeAnimation', [
            transition(':increment', [
                style({ height: '!' }),
                query(':enter', style({ transform: 'translateX(100%)' })),
                query(':enter, :leave', style({ position: 'absolute', top: 0, left: 0, right: 0 })),
                // animate the leave page away
                group([
                    query(':leave', [
                        animate('0.3s cubic-bezier(.35,0,.25,1)', style({ transform: 'translateX(-100%)' })),
                    ]),
                    // and now reveal the enter
                    query(':enter', animate('0.3s cubic-bezier(.35,0,.25,1)', style({ transform: 'translateX(0)' }))),
                ]),
            ]),
            transition(':decrement', [
                style({ height: '!' }),
                query(':enter', style({ transform: 'translateX(-100%)' })),
                query(':enter, :leave', style({ position: 'absolute', top: 0, left: 0, right: 0 })),
                // animate the leave page away
                group([
                    query(':leave', [
                        animate('0.3s cubic-bezier(.35,0,.25,1)', style({ transform: 'translateX(100%)' })),
                    ]),
                    // and now reveal the enter
                    query(':enter', animate('0.3s cubic-bezier(.35,0,.25,1)', style({ transform: 'translateX(0)' }))),
                ]),
            ]),
        ])
    ]
})

export class AppComponent implements OnInit {
    // Screen info
    screenHeight: any;
    screenWidth: any;

    jwtHelper = new JwtHelperService();

    constructor(public authService: AuthService, public settingsService: SettingsService) { }

    ngOnInit() {
        const token = localStorage.getItem('token');
        if (token) {
            this.authService.decodedToken = this.jwtHelper.decodeToken(token);
        }

        this.onResize();

        if (this.authService.loggedIn()) {
            this.settingsService.loadUI();
        }
    }

    loggedIn() {
        return this.authService.loggedIn();
    }

    getMainDepth(outlet) {
        return outlet.activatedRouteData['mainDepth'];
    }

    // Help displaying
    @HostListener('window:resize', ['$event'])
    onResize(event?) {
        this.screenHeight = window.innerHeight;
        this.screenWidth = window.innerWidth;
    }
}
