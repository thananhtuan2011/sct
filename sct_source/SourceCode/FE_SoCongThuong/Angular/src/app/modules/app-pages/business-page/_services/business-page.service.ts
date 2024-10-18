import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BusinessPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Business`;
  _AuthService : AuthService

  constructor(
    @Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  createFormData(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL, item,{ headers: httpHeaders});
  }

  updateFormData(item: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders_FormData();
    const url = `${this.API_URL}/${item.id}`;
    return this.http.put<any>(url, item,{ headers: httpHeaders});
  }

  public viewinfo(id: any) {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/User/' + `${id}`,
      {
        headers: httpHeaders
      });
  }

  public loadIndustries() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadindustries',
      {
        headers: httpHeaders
      });
  }

  public loadTypeOfBusinesses() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadtypeofbusinesses',
      {
        headers: httpHeaders
      });
  }

  public loadTypeOfProfession() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/loadtypeofprofession',
      {
        headers: httpHeaders
      });
  }

  deleteBusiness(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteBusiness/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  deleteBusinesses(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deleteBusinesses`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
