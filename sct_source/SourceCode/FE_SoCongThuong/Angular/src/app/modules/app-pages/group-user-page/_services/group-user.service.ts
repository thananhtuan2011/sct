import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GroupUserService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/Permission`;
  _AuthService:AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
    this._AuthService = _AuthService
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  GetTreePhanQuyen(id:number): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.post<any>(this.API_URL + '/LayTreePhanQuyen?IdGroup='+id,null, { headers: httpHeaders });
	}
  updateQuyenNhomNguoiDung(item: any){
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.post<any>(this.API_URL + '/UserGroupRoles_Update', item, { headers: httpHeaders });
	}
}
