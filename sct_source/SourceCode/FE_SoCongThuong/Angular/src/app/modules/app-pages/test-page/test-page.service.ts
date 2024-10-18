import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { QueryParamsModel, QueryResultsModel } from '../../auth/models/query-params.model';
import { AuthService } from '../../auth/services/auth.service';
import { DemoModel } from './test-page-models/test-page-model';

const API_URL = `${environment.apiUrl}/demo`;

@Injectable({
  providedIn: 'root',
})
export class TestPageService {
  lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 5));
  constructor(
    private http: HttpClient,
    private _AuthService: AuthService
  ) { }
  getData(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    const httpParams = this._AuthService.getFindHTTPParams(queryParams);
    return this.http.get<any>(API_URL + '/getlist',
      {
        headers: httpHeaders,
        params: httpParams
      });
  }
  addupdate(item: DemoModel): Observable<any> {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.post<any>(API_URL + '/add-edit', item, { headers: httpHeaders });
  }
  delete(id: number): Observable<any> {
    const httpHeaders = this._AuthService.getHTTPHeaders();
    return this.http.get<any>(API_URL + `/delete?id=${id}`, { headers: httpHeaders });
  }
}
