import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProvincialTradeStatisticsPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Statistical`;
  _AuthService : AuthService;

  constructor(
    @Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  public loadData() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/StatisticalByProvince',
      {
        headers: httpHeaders
      }
    );
  }
}
