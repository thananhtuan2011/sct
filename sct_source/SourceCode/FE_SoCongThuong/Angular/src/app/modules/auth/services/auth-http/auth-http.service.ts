import { Injectable, OnDestroy } from '@angular/core';
import { Observable, Subscription, map } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserModel } from '../../models/user.model';
import { environment } from '../../../../../environments/environment';
import { AuthService } from '../auth.service';

const API_USERS_URL = `${environment.apiUrl}/User`;

@Injectable({
  providedIn: 'root',
})
export class AuthHTTPService implements OnDestroy {
  ip: string = '';
  clientIpAddress: any;
  private unsubscribe: Subscription[] = [];
  constructor(
    private http: HttpClient
) { 
  const ip = this.getIpAddress().subscribe((res: any) => this.ip = res);
  this.unsubscribe.push(ip);
}

ngOnDestroy() {
  this.unsubscribe.forEach((sb) => sb.unsubscribe());
}

  // public methods
  login(email: string, password: string, recaptchatoken: string): Observable<any> {
    const httpHeaders = new HttpHeaders({
      'X-Forwarded-For': this.ip,
    });
    return this.http.post<any>(`${API_USERS_URL}/login`, {
      UserName: email,
      PassWord: password,
      RecaptchaToken: recaptchatoken
    },
    {
      headers: httpHeaders
    });
  }
  
  getIpAddress (): Observable<any>{
    const apiUrl = 'https://api.ipify.org?format=json'
    return this.http.get(apiUrl).pipe(map((res: any) => res.ip));
  }

  // CREATE =>  POST: add a new user to the server
  createUser(user: UserModel): Observable<UserModel> {
    return this.http.post<UserModel>(API_USERS_URL, user);
  }

  // Your server should check email => If email exists send link to the user and return true | If email doesn't exist return false
  forgotPassword(email: string): Observable<boolean> {
    return this.http.post<boolean>(`${API_USERS_URL}/forgot-password`, {
      email,
    });
  }

  getUserByToken(token: string): Observable<any> {
    const httpHeaders = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<any>(`${API_USERS_URL}/getUserByToken`, {}, {
      headers: httpHeaders,
    });
  }
}
