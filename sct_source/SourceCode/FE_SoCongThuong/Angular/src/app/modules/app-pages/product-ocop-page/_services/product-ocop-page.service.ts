import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductOcopPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/ProductOcop`;
  _AuthService : AuthService

  constructor(
    @Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  createformdata(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL, item,{ headers: httpHeaders});
  }

  updateformdata(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    const url = `${this.API_URL}/${item.id}`;
    return this.http.put<any>(url, item,{ headers: httpHeaders});
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  delete(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/delete/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  deletes(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deletes`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
