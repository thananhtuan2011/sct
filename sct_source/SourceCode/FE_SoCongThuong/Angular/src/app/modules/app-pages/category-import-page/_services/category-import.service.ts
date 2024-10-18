import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryImportService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/import`;
  _AuthService : AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  importFileExcel(item:any) {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL + '/ImportExcel', item,
    {
      headers: httpHeaders
    });
  }
  importFileExcelSave(item:any) {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL + '/ImportExcelSave', item,
    {
      headers: httpHeaders
    });
  }

  loadFileExcel(item:any) {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.post<any>(this.API_URL + '/GetListColumns', item,
    {
      headers: httpHeaders
    });
  }

  loadListDanhMuc() {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.get<any>(this.API_URL + '/list-danh-muc',
    {
      headers: httpHeaders
    });
  }

  loadItemMucByUrl(id:any) {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.get<any>(this.API_URL + `/get-item-by-url?id=${id}`,
    {
      headers: httpHeaders
    });
  }

  loadColsDanhMucById(id:any) {
    const httpHeaders = this._AuthService.getHTTPHeaders_FormData();
    return this.http.get<any>(this.API_URL + `/get-danh-muc-by-id?id=${id}`,
    {
      headers: httpHeaders
    });
  }
}
