import { AuthService } from 'src/app/modules/auth';
// tslint:disable:variable-name
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, map, tap } from 'rxjs/operators';
import { PaginatorState } from '../models/paginator.model';
import { ITableState, TableResponseModel } from '../models/table.model';
import { BaseModel } from '../models/base.model';
import { SortState } from '../models/sort.model';
import { GroupingState } from '../models/grouping.model';
import { environment } from '../../../../../environments/environment';
import Swal from 'sweetalert2';

// const DEFAULT_STATE: ITableState = {
//   filter: {},
//   paginator: new PaginatorState(),
//   sorting: new SortState(),
//   searchTerm: '',
//   grouping: new GroupingState(),
//   entityId: undefined
// };
export abstract class TableService<T> {
  // Private fields
  private _items$ = new BehaviorSubject<T[]>([]);
  private _isLoading$ = new BehaviorSubject<boolean>(false);
  private _isFirstLoading$ = new BehaviorSubject<boolean>(true);
  private _tableState$ = new BehaviorSubject<ITableState>(
    {
      filter: {},
      paginator: new PaginatorState(),
      sorting: new SortState(),
      searchTerm: '',
      grouping: new GroupingState(),
      entityId: undefined
    }
  );
  private _errorMessage = new BehaviorSubject<string>('');
  private _subscriptions: Subscription[] = [];

  // Getters
  get items$() {
    return this._items$.asObservable();
  }
  get isLoading$() {
    return this._isLoading$.asObservable();
  }
  get isFirstLoading$() {
    return this._isFirstLoading$.asObservable();
  }
  get errorMessage$() {
    return this._errorMessage.asObservable();
  }
  get subscriptions() {
    return this._subscriptions;
  }
  // State getters
  get paginator() {
    return this._tableState$.value.paginator;
  }
  get filter() {
    return this._tableState$.value.filter;
  }
  get sorting() {
    return this._tableState$.value.sorting;
  }
  get searchTerm() {
    return this._tableState$.value.searchTerm;
  }
  get grouping() {
    return this._tableState$.value.grouping;
  }
  // protected http: HttpClient;
  // API URL has to be overrided
  abstract API_URL: string;
  protected http:HttpClient
  protected auth:AuthService
  constructor( http: HttpClient,
    _AuthService: AuthService) {
    this.http = http;
    this.auth = _AuthService
  }

  // CREATE
  // server should return the object with ID
  create(item: BaseModel): Observable<BaseModel> {
    const httpHeaders = this.auth.getHTTPHeaders();
    this._isLoading$.next(true);
    this._errorMessage.next('');
    return this.http.post<BaseModel>(this.API_URL, item,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('CREATE ITEM', err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  // READ (Returning filtered list of entities)
  find(tableState: ITableState): Observable<TableResponseModel<T>> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = this.API_URL + '/find';
    this._errorMessage.next('');
    return this.http.post<TableResponseModel<T>>(url, tableState,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('FIND ITEMS', err);
        if (err.error === "Không có dữ liệu") {
          return of({ items: [], total: 0 });
        }
        else {
          Swal.fire({
            icon: 'error',
            title: 'Thông báo lỗi',
            confirmButtonText: 'Xác nhận',
            text: err.error.error.msg,
          });
          return of({ items: [], total: 0 });
        }
      })
    );
  }

  getItemById(id: number): Observable<BaseModel> {
    const httpHeaders = this.auth.getHTTPHeaders();
    this._isLoading$.next(true);
    this._errorMessage.next('');
    const url = `${this.API_URL}/${id}`;
    return this.http.get<BaseModel>(url,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('GET ITEM BY IT', id, err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  // UPDATE
  update(item: BaseModel): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    const url = `${this.API_URL}/${item.id}`;
    this._isLoading$.next(true);
    this._errorMessage.next('');
    return this.http.put(url, item,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('UPDATE ITEM', item, err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  // UPDATE Status
  updateStatusForItems(ids: number[], status: number): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    this._isLoading$.next(true);
    this._errorMessage.next('');
    const body = { ids, status };
    const url = this.API_URL + '/updateStatus';
    return this.http.put(url, body,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('UPDATE STATUS FOR SELECTED ITEMS', ids, status, err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  // DELETE
  delete(id: any): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    this._isLoading$.next(true);
    this._errorMessage.next('');
    const url = `${this.API_URL}/${id}`;
    return this.http.delete(url,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('DELETE ITEM', id, err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  // delete list of items
  deleteItems(ids: number[] = []): Observable<any> {
    const httpHeaders = this.auth.getHTTPHeaders();
    this._isLoading$.next(true);
    this._errorMessage.next('');
    const url = this.API_URL + '/deleteItems';
    const body = { ids };
    return this.http.put(url, body,{ headers: httpHeaders}).pipe(
      catchError(err => {
        this._errorMessage.next(err);
        console.error('DELETE SELECTED ITEMS', ids, err);
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
          text: err.error.error.msg,
        });
        return of(err.error);
      }),
      finalize(() => this._isLoading$.next(false))
    );
  }

  public fetch() {
    this._isLoading$.next(true);
    this._errorMessage.next('');
    const request = this.find(this._tableState$.value)
      .pipe(
        tap((res: TableResponseModel<T>) => {
          this._items$.next(res.items);
          this.patchStateWithoutFetch({
            paginator: this._tableState$.value.paginator.recalculatePaginator(
              res.total
            ),
          });
        }),
        catchError((err) => {
          this._errorMessage.next(err);
          return of({
            items: [],
            total: 0
          });
        }),
        finalize(() => {
          this._isLoading$.next(false);
          const itemIds = this._items$.value.map((el: T) => {
            const item = (el as unknown) as BaseModel;
            return item.id;
          });
          this.patchStateWithoutFetch({
            grouping: this._tableState$.value.grouping.clearRows(itemIds),
          });
        })
      )
      .subscribe();
    this._subscriptions.push(request);
  }

  public setDefaults() {
    this.patchStateWithoutFetch({ filter: {} });
    this.patchStateWithoutFetch({ sorting: new SortState() });
    this.patchStateWithoutFetch({ grouping: new GroupingState() });
    this.patchStateWithoutFetch({ searchTerm: '' });
    this.patchStateWithoutFetch({
      paginator: new PaginatorState()
    });
    this._isFirstLoading$.next(true);
    this._isLoading$.next(true);
    this._tableState$.next(
      {
        filter: {},
        paginator: new PaginatorState(),
        sorting: new SortState(),
        searchTerm: '',
        grouping: new GroupingState(),
        entityId: undefined
      }
    );
    this._errorMessage.next('');
  }

  // Base Methods
  public patchState(patch: Partial<ITableState>) {
    this.patchStateWithoutFetch(patch);
    this.fetch();
  }
  
  public patchStateWithoutFetch(patch: Partial<ITableState>) {
    const newState = Object.assign(this._tableState$.value, patch);
    this._tableState$.next(newState);
  }
}
