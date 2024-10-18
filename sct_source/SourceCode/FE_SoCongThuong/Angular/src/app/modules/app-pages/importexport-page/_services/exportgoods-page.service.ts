import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExportGoodsPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/ExportGoods`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  public loadItemGroup() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loaditemgroup',
      {
        headers: httpHeaders
      });
  }

  public loadTypeOfEconomic() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadtypeofeconomic',
      {
        headers: httpHeaders
      });
  }

  public loadCountry() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadcountry',
      {
        headers: httpHeaders
      });
  }

  public loadBusiness() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadbusiness',
      {
        headers: httpHeaders
      });
  }

  public loadUnit() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadunits',
      {
        headers: httpHeaders
      });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteExportGood(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteExportGood/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  deleteExportGoods(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deleteExportGoods`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
