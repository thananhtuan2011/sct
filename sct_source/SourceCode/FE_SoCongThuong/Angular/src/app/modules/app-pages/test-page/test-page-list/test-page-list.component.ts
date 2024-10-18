import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { LiveAnnouncer } from '@angular/cdk/a11y';
import { QueryParamsModel } from 'src/app/modules/auth/models/query-params.model';
import { TestPageDataSource } from './test-page.datasource';
import { TestPageService } from '../test-page.service';
import { debounceTime, distinctUntilChanged, fromEvent, merge, tap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { TestPageModalComponent } from '../test-page-modal/test-page-modal.component';
import { DemoModel } from '../test-page-models/test-page-model';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-test-page-list',
  templateUrl: './test-page-list.component.html',
})
export class TestPageListComponent implements OnInit {

  constructor(
    private _liveAnnouncer: LiveAnnouncer,
    private detectchanges: ChangeDetectorRef,
    private _TestPageService: TestPageService,
    private route: ActivatedRoute,
    private modalService: NgbModal
  ) { }

  displayedColumns: string[] = ['Id', 'Name', 'actions'];
  dataSource: TestPageDataSource;
  Result: any[] = [];
  sortactive = "Id";

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild('sort1', { static: true }) sort: MatSort;
  @ViewChild('searchInput', { static: true }) searchInput: ElementRef;
  modalReference: NgbModalRef;
  ngOnInit(): void {

    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => {
          this.LoadList();
        })
      )
      .subscribe();

    fromEvent(this.searchInput.nativeElement, 'keyup')
      .pipe(
        debounceTime(150),
        distinctUntilChanged(),
        tap(() => {
          this.paginator.pageIndex = 0;
          this.LoadList();
        })
      )
      .subscribe();
    this.dataSource = new TestPageDataSource(this._TestPageService);
    let queryParams = new QueryParamsModel({});
    queryParams.sortField = this.sortactive;
    this.route.queryParams.subscribe(params => {
      if (params.id) {
        queryParams = this._TestPageService.lastFilter$.getValue();
      }
      this.dataSource.LoadList(queryParams);
    });
    this.dataSource.entitySubject.subscribe(res => this.Result = res);
    this.detectchanges.detectChanges();
  }
  LoadList() {
    const queryParams = new QueryParamsModel(
      this.filterConfiguration(),
      this.sort.direction,
      this.sort.active,
      this.paginator.pageIndex,
      this.paginator.pageSize
    );
    this.dataSource.LoadList(queryParams);
  }
  Title(): string {
    return "Demo page";
  }
  filterConfiguration(): any {
    const filter: any = {};
    const searchText: string = this.searchInput.nativeElement.value;
    filter.flt_search = searchText;
    return filter;
  }
  add() {
    let data: DemoModel = new DemoModel();
    this.OpenModal(data);
  }
  edit(data: DemoModel) {
    this.OpenModal(data);
  }
  OpenModal(item: DemoModel) {
    const modalRef = this.modalService.open(
      TestPageModalComponent,
      {
        size: 'lg'
      }
    );
    modalRef.componentInstance.data = item;
    modalRef.result.then(
      (result) => {
        if (result) {
          Swal.fire({
            icon: result.status == 1 ? 'success' : 'error',
            title: result.status == 1 ? 'Thành công' : 'Thất bại',
            confirmButtonText: 'Xác nhận',
            text: (item.id > 0 ? 'Cập nhật ' : 'Thêm mới ') + (result.status == 1 ? 'thành công' : 'thất bại'),
          });
          this.LoadList();
        }
      },
      () => { });
  }
  delete(item: DemoModel) {
    Swal.fire({
      title: 'Bạn có muốn xóa ' + item.name + ' ?',
      text: 'Hành động này không thể hoàn tác!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this._TestPageService.delete(item.id).subscribe(res => {
          Swal.fire({
            icon: res.status == 1 ? 'success' : 'error',
            title: res.status == 1 ? 'Thành công' : 'Thất bại',
            confirmButtonText: 'Xác nhận',
            text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
          });
          this.LoadList();
        });
      }
    })
  }
}