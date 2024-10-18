import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CateIntegratedManagementPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/CateIntegratedManagement`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  public viewinfo(id: any) {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/info/' + `${id}`,
      {
        headers: httpHeaders
      });
  }

  // public viewdisbursement(id: any) {
  //   const httpHeaders = this._AuthService.getHTTPHeaders();
  //   return this.http.get<any>(this.API_URL + '/info/' + `${id}`,
  //     {
  //       headers: httpHeaders
  //     });
  // }

  // public viewhistory(id: any) {
  //   const httpHeaders = this._AuthService.getHTTPHeaders();
  //   return this.http.get<any>(this.API_URL + '/info/' + `${id}`,
  //     {
  //       headers: httpHeaders
  //     });
  // }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
}
