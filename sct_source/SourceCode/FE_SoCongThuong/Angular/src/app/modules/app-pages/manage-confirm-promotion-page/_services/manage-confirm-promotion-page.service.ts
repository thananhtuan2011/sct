import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ManageConfirmPromotionPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/ManageConfirmPromotion`;
  _AuthService: AuthService
  constructor(@Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http, _AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteManageConfirmPromotion(id: string): Observable<any> {
    // console.log('delete: ', id);
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteManageConfirmPromotion/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }

  // deleteManageConfirmPromotions(ids: any[] = []): Observable<any> {
  //   const httpHeaders = this.auth.getHTTPHeaders();
  //   const body = { districtIds: ids };
  //   const url = `${this.API_URL}/deleteManageConfirmPromotions`;
  //   return this.http.put(url, body, { headers: httpHeaders });
  // }

  createformdata(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL, item, { headers: httpHeaders });
  }

  updateformdata(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    const url = `${this.API_URL}/${item.id}`;
    return this.http.put<any>(url, item, { headers: httpHeaders });
  }

  public loadUser() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadUser',
      {
        headers: httpHeaders
      });
  }
}
