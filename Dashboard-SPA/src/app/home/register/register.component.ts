import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  username = new FormControl();
  password = new FormControl();

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private activeRoute: ActivatedRoute
    ) { }

  ngOnInit() {
  }

  register() {
    this.model.username = this.username.value;
    this.model.password = this.password.value;

    this.authService.register(this.model).subscribe(
      () => {
        this.router.navigate(['/home'], {fragment: 'login'});
        this.username.reset();
        this.password.reset();
        this.alertify.success('Registration successful!');
      },
      error => {
        this.alertify.error(error);
      }
    );
  }
}
