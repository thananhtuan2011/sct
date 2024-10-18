import { Injectable, OnDestroy } from '@angular/core';
import { Observable, BehaviorSubject, of, Subscription, merge, fromEvent } from 'rxjs';
import { map, catchError, switchMap, finalize } from 'rxjs/operators';
import { UserModel, UserLoginFailModel } from '../models/user.model';
import { AuthModel } from '../models/auth.model';
import { AuthHTTPService } from './auth-http';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { QueryParamsModel } from '../models/query-params.model';
import { CookieService } from 'ngx-cookie-service';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
export type UserType = UserModel | UserLoginFailModel | undefined;
const DOMAIN = environment.DOMAIN_COOKIES;
@Injectable({
  providedIn: 'root',
})
export class AuthService implements OnDestroy {
  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/
  private authLocalStorageToken = `${environment.appVersion}-${environment.USERDATA_KEY}`;

  // public fields
  currentUser$: Observable<UserType>;
  isLoading$: Observable<boolean>;
  currentUserSubject: BehaviorSubject<any>;
  isLoadingSubject: BehaviorSubject<boolean>;
  networkStatus: boolean = false;
  networkStatus$: Subscription = Subscription.EMPTY;
  //Kiểm tra kết nối internet
  networkOff: boolean = false;
  //show dialog
  horizontalPosition: MatSnackBarHorizontalPosition = 'left';
  verticalPosition: MatSnackBarVerticalPosition = 'bottom';

  get currentUserValue(): UserType {
    return this.currentUserSubject.value;
  }

  set currentUserValue(user: UserType) {
    this.currentUserSubject.next(user);
  }
  ip: string = '';

  constructor(
    private authHttpService: AuthHTTPService,
    private router: Router,
    private http: HttpClient,
    private cookieService: CookieService,
    private _snackBar: MatSnackBar,
    
  ) {
    this.isLoadingSubject = new BehaviorSubject<boolean>(false);
    this.currentUserSubject = new BehaviorSubject<UserType>(undefined);
    this.currentUser$ = this.currentUserSubject.asObservable();
    this.isLoading$ = this.isLoadingSubject.asObservable();
    const subscr = this.getUserByToken().subscribe();
    const ip = this.getIpAddress().subscribe((res: any) => this.ip = res);
    this.unsubscribe.push(subscr, ip);
    this.networkStatus = navigator.onLine;
    this.networkStatus$ = merge(
      of(null),
      fromEvent(window, 'online'),
      fromEvent(window, 'offline')
    )
      .pipe(map(() => navigator.onLine))
      .subscribe(status => {
        if (!status) {
          //show dialog
          this._snackBar.open('Bạn đang offline.', 'Đóng', {
            horizontalPosition: this.horizontalPosition,
            verticalPosition: this.verticalPosition,
          });
          this.networkOff = true
        } else {
          if (this.networkOff) {
            //show dialog
            this._snackBar.open('Đã khôi phục kết nối Internet.', 'Đóng', {
              horizontalPosition: this.horizontalPosition,
              verticalPosition: this.verticalPosition,
              duration: 5000
            });
          }
        }
        this.networkStatus = status;
      });
    if (window.navigator.onLine) {
      setInterval(() => {
        if (this.networkStatus) {
          this.getUserByToken().subscribe();
        }
      }, 5000);
    }
  }
  public getDataUser(){
    const auth = this.currentUserSubject.value;
    return auth;
  }
  // public methods
  login(email: string, password: string, recaptchatoken: string): Observable<UserType> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.login(email, password, recaptchatoken).pipe(
      map((auth) => {
        if (auth && auth.access_token && auth.refresh_token) {
          //this.currentUserSubject.next(auth);
          this.saveToken_cookie(auth.access_token, auth.refresh_token);
        }
        const result = this.setAuthFromLocalStorage(auth);
        return result;
      }),
      switchMap(() => this.getUserByToken()),
      catchError((err) => {
        console.error('err', err);
        if (err.error.countLoginFail) {
          return of(err.error)
        }
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  saveToken_cookie(access_token?: string, refresh_token?: string) {
    if (access_token) this.cookieService.set('access_token', access_token, 365, '/', DOMAIN);
    if (refresh_token) this.cookieService.set('refresh_token', refresh_token, 365, '/', DOMAIN);
  }

  deleteAccessRefreshToken_cookie() {
    this.cookieService.delete('access_token', '/', DOMAIN);
    this.cookieService.delete('refresh_token', '/', DOMAIN);
  }

  getAccessToken_cookie() {
    const access_token = this.cookieService.get('access_token');
    return access_token;
  }

  logout(): Observable<any>{
    const httpHeaders = this.getHTTPHeaders();
    
    const logout =  this.http.post<any>(`${environment.apiUrl}/User/logout`, {},
      {
        headers: httpHeaders,
      }
    );
    this.deleteAccessRefreshToken_cookie();
    localStorage.removeItem(this.authLocalStorageToken);
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login'], {
      queryParams: {},
    });
    return logout;
  }
    
  getIpAddress (): Observable<any>{
    const apiUrl = 'https://api.ipify.org?format=json'
    return this.http.get(apiUrl).pipe(map((res: any) => res.ip));
  }

  getUserByToken(): Observable<UserType> {
    const access_token = this.getAccessToken_cookie();
    if (!access_token) {
      this.logout();
      return of(undefined);
    }

    this.isLoadingSubject.next(true);
    return this.authHttpService.getUserByToken(access_token).pipe(
      map((user) => {
        if (user) {
          if(user.data){
            this.currentUserSubject.next(user.data);
          }
        }
        return user;
      }),
      catchError(err => {
        if(err.status == 401){
          this.logout();
        }
        return of(err);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  // need create new user then login
  registration(user: UserModel): Observable<any> {
    this.isLoadingSubject.next(true);
    return this.authHttpService.createUser(user).pipe(
      map(() => {
        this.isLoadingSubject.next(false);
      }),
      switchMap(() => this.login(user.email, user.password, "")),
      catchError((err) => {
        console.error('err', err);
        return of(undefined);
      }),
      finalize(() => this.isLoadingSubject.next(false))
    );
  }

  forgotPassword(email: string): Observable<boolean> {
    this.isLoadingSubject.next(true);
    return this.authHttpService
      .forgotPassword(email)
      .pipe(finalize(() => this.isLoadingSubject.next(false)));
  }

  // private methods
  private setAuthFromLocalStorage(auth: AuthModel): boolean {
    // store auth authToken/refreshToken/epiresIn in local storage to keep user logged in between page refreshes
    if (auth) {
      localStorage.setItem(this.authLocalStorageToken, JSON.stringify(auth));
      return true;
    }
    return false;
  }

  public getRoles(){
    const auth = this.currentUserSubject.value;
    return auth.roles;
  }

  private getAuthFromLocalStorage(): AuthModel | undefined|any {
    try {
      const lsValue = localStorage.getItem(this.authLocalStorageToken);
      if (!lsValue) {
        return undefined;
      }

      const authData = JSON.parse(lsValue);
      return authData;
    } catch (error) {
      console.error(error);
      return undefined;
    }
  }
  getMenu(): Observable<any> {
    const httpHeaders = this.getHTTPHeaders();
    return this.http.post<any>(`${environment.apiUrl}/User/getMenu`, {},
      {
        headers: httpHeaders,
      }
    );
  }
  getHTTPHeaders(): HttpHeaders {
    const access_token = this.getAccessToken_cookie();
    const httpHeaders = new HttpHeaders({
      'Authorization': `Bearer ${access_token}`,
      'Content-Type': `application/json`,
      'TimeZone': (new Date()).getTimezoneOffset().toString(),
      'X-Forwarded-For': this.ip
    });
    return httpHeaders;
  }
  getHTTPHeaders_FormData(): HttpHeaders {
    const auth = this.getAccessToken_cookie();
    let result = new HttpHeaders({
      'Accept': 'application/json',
      'Authorization': `Bearer ${auth}`,
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Headers': 'Accept',
      'TimeZone': (new Date()).getTimezoneOffset().toString(),
    });
    return result;
  }
  getFindHTTPParams(queryParams:any): HttpParams {
		let params = new HttpParams()
		.set('sortOrder', queryParams.sortOrder)
		.set('sortField', queryParams.sortField)
		.set('page', (queryParams.pageNumber + 1).toString())
		.set('record', queryParams.pageSize.toString());
		let keys:any = [];
    let values:any = [];
		if (queryParams.more) {
			params = params.append('more', 'true');
		}
		Object.keys(queryParams.filter).forEach(function (key) {
		if (typeof queryParams.filter[key] !== 'string' || queryParams.filter[key] !== '') {
		keys.push(key);
		values.push(queryParams.filter[key]);
		}
		});
		if (keys.length > 0) {
		params = params.append('filter.keys', keys.join('|'))
		.append('filter.vals', values.join('|'));
		}
		let grp_keys:any = [], grp_values:any = [];
		if(queryParams.filterGroup)
		{
			Object.keys(queryParams.filterGroup).forEach(function (key) {
			if (typeof queryParams.filterGroup[key] !== 'string' || queryParams.filterGroup[key] !== '') {
				grp_keys.push(key);
				let value_str = "";
				if (queryParams.filterGroup[key] && queryParams.filterGroup[key].length > 0) {
					value_str = queryParams.filterGroup[key].join(',')
				}
				grp_values.push(value_str);
			}
			});
			if (grp_keys.length > 0) {
				params = params.append('filterGroup.keys', grp_keys.join('|'))
				.append('filterGroup.vals', grp_values.join('|'));
			}
		}
		return params;
		}
  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
