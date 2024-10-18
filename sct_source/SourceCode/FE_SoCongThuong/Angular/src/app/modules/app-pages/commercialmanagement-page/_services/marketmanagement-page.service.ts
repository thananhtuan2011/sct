import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MarketManagementPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/MarketManagement`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  public loadDistrict() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loaddistrict',
      {
        headers: httpHeaders
      });
  }

  public loadCommune() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadcommune',
      {
        headers: httpHeaders
      });
  }

  public loadMarket() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadmarket',
      {
        headers: httpHeaders
      });
  }

  public loadBusinessLine() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadbusinessline',
      {
        headers: httpHeaders
      });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteMarketManagement(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteMarketManagement/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }

  deleteMarketManagements(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deleteMarketManagements`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
  
  getBusinessLine(): Observable<any>{
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${environment.apiUrl}/BusinessLine/getAllBusinessLine`;
    return this.http.get(url, { headers: httpHeaders });
  }
}
