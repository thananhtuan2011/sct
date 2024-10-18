import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs/internal/Observable';

@Injectable({
  providedIn: 'root'
})
export class IndustryPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Industry`;
  _AuthService: AuthService
  constructor(@Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  public loadIndustry(id: any) {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(`${this.API_URL}/loadIndustry/${id}`,
      {
        headers: httpHeaders
      });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteIndustry(id: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteIndustry/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }

  deleteIndustries(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { IndustryIds: ids };
    const url = `${this.API_URL}/deleteItems`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
