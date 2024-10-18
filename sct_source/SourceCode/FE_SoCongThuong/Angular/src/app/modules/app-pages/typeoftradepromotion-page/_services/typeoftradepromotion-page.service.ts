import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TypeOfTradePromotionPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/TypeOfTradePromotion`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  deleteTypeOfTradePromotion(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/deleteTypeOfTradePromotion/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
  deleteTypeOfTradePromotions(ids: any[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const body = { districtIds: ids };
    const url = `${this.API_URL}/deleteTypeOfTradePromotions`;
    return this.http.put(url, body, { headers: httpHeaders });
  }
}
