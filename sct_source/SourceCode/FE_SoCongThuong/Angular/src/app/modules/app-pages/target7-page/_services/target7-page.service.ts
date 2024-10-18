import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Target7PageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Target7`;
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

  loadDistrict(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/LoadDistrict', { headers: httpHeaders });
	}

  loadCommune(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/LoadCommune', { headers: httpHeaders });
	}

  loadStage(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/LoadStage', { headers: httpHeaders });
	}

  delete(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/delete/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  loadDataProvice(query: any): Observable<any>{
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/GetDataProvice`;
    return this.http.post(url, query ,{ headers: httpHeaders });
  }
}
