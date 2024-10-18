import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommercialManagementPageService extends TableService<any> implements OnDestroy {
  check_form = new Subject();
  API_URL = `${environment.apiUrl}/CommercialManagement`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
  _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  public loadTypeOfMarket() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadtypeofmarket',
      {
        headers: httpHeaders
      });
  }

  public loadCategory() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadcategory',
      {
        headers: httpHeaders
      });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteCommercialManagement(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteCommercialManagement/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  deleteCommercialManagements(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deleteCommercialManagements`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
