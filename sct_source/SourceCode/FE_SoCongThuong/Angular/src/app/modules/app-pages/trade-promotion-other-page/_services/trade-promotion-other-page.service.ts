import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { catchError, finalize, Observable, of } from 'rxjs';
import { BaseModel } from 'src/app/modules/auth/models/query-params.model';

@Injectable({
  providedIn: 'root'
})
export class TradePromotionOtherPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/TradePromotionOther`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
  delete(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/delete`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
  createformdata(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL, item,{ headers: httpHeaders});
  }
    // UPDATE
  updateformdata(item: any,id:any): Observable<any> {
      const httpHeaders = this.auth.getHTTPHeaders_FormData();
      const url = `${this.API_URL}/${id}`;
      return this.http.put(url, item,{ headers: httpHeaders});
    }
  deleteRP(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/delete/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  public loadBusinesses() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadbusinesses',
      {
        headers: httpHeaders
      });
  }
}
