import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Dashboard`;
  _AuthService: AuthService

  constructor(
    @Inject(HttpClient) http: any,
    _AuthService: AuthService) {
    super(http, _AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  public loadDataStatus() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataStatus',
      {
        headers: httpHeaders
      });
  }

  public loadDataMarket() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataMarket',
      {
        headers: httpHeaders
      });
  }

  public loadDistrict() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDistrict',
      {
        headers: httpHeaders
      });
  }

  public loadDataFunding() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataFunding',
      {
        headers: httpHeaders
      });
  }

  public loadDataAlcoholProduct() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataAlcoholProduct',
      {
        headers: httpHeaders
      });
  }

  public loadDataPetroleum() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataPetroleum',
      {
        headers: httpHeaders
      });
  }

  public loadDataBusiness() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataBusiness',
      {
        headers: httpHeaders
      });
  }

  public loadAlcohol() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getAlcohol',
      {
        headers: httpHeaders
      });
  }

  public loadCigarette() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataCigarette',
      {
        headers: httpHeaders
      });
  }

  public loadImportExportGoods() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataVolumeExportImport',
      {
        headers: httpHeaders
      });
  }

  public loadBusinessName() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getBusinessName',
      {
        headers: httpHeaders
      });
  }

  public loadTradePromotionProjectManagement() {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getDataTradePromotionProjectManagement',
      {
        headers: httpHeaders
      });
  }

  public loadExImpordDataById(id: any) {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(this.API_URL + '/getExImpordDataById/' + id,
      {
        headers: httpHeaders
      });
  }
}
