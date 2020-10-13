import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    baseUrl = environment.apiUrl + 'auth/';
    jwtHelper = new JwtHelperService();
    decodedToken: any;

    constructor(private http: HttpClient) { }

    login(model: any) {
        // Make HTTP POST request to API login method
        return this.http.post(this.baseUrl + 'login', model)
            // Store login token (with chained/piped rxjs operators)
            .pipe(
                map((response: any) => {
                    if (response) {
                        localStorage.setItem('token', response.token);
                        this.decodedToken = this.jwtHelper.decodeToken(response.token);
                    }
                })
            );
    }

    loggedIn() {
        const token = localStorage.getItem('token');
        return !this.jwtHelper.isTokenExpired(token);
    }

    register(model: any) {
        return this.http.post(this.baseUrl + 'register', model);
    }

    isAdmin() {
        if (this.decodedToken.role === 'admin') {
            return true;
        }
        return false;
    }
}
