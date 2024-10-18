import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/User`;
  _AuthService:AuthService

  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }
  changePassword(item:any): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.post<any>(this.API_URL + '/change-password-user-current', item, { headers: httpHeaders });
	}


  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
}
