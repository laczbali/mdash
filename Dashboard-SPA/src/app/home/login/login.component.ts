import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SettingsService } from 'src/app/_services/settings.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  model: any = {};

  constructor(
    private router: Router,
    private authService: AuthService,
    private alertify: AlertifyService,
    private settingsService: SettingsService
  ) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Logged in!');
        this.settingsService.loadUI();
        this.router.navigate(['/overview']);
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
