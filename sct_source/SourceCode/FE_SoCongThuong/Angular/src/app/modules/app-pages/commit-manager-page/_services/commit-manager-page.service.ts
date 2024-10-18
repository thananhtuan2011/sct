import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { List } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class CommitManagerPageService extends TableService<any> implements OnDestroy {
  API_URL = `${environment.apiUrl}/CommitManager`;
  protected auth:AuthService
  constructor(@Inject(HttpClient) http: any,
   _AuthService: AuthService) {
    super(http,_AuthService);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }
  
  delete(id: string): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/delete/${id}`;
    return this.http.put(url, {} ,{ headers: httpHeaders });
  }
  
}
