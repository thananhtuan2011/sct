import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BusinessLinePageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/BusinessLine`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteBusinessLine(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteBusinessLine/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }

}
