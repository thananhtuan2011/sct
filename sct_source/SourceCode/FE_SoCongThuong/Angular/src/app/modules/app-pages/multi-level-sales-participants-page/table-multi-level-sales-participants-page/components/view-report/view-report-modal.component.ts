import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import * as moment from 'moment';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import { MultiLevelSalesParticipantsModel } from '../../../_models/multi-level-sales-participants.model';
import { MultiLevelSalesParticipantsPageService } from '../../../_services/multi-level-sales-participants-page.service';
import { PaginatorState } from 'src/app/_metronic/shared/crud-table/models/paginator.model';
import { SortState } from 'src/app/_metronic/shared/crud-table/models/sort.model';
import { GroupingState } from 'src/app/_metronic/shared/crud-table/models/grouping.model';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-view-report-modal',
  templateUrl: './view-report-modal.component.html',
  styleUrls: ['./view-report-modal.component.scss'],

})
export class ViewReportMultiLevelSalesParticipantsModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  multiLevelSalesParticipantsData: MultiLevelSalesParticipantsModel;
  formGroup: FormGroup;
  options: Options;
  filterGroup: FormGroup;

  private subscriptions: Subscription[] = [];
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  MinDate: any = null;
  MaxDate: any = null;
  filterValue: any = {}

  constructor(
    public multiLevelSalesParticipantsService: MultiLevelSalesParticipantsPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private http: HttpClient
  ) { }
  ngOnDestroy(): void {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
  }

  ngOnInit(): void {
    this.filterForm();
    this.isLoading$ = this.multiLevelSalesParticipantsService.isLoading$;
    this.multiLevelSalesParticipantsService.fetch();
    this.grouping = this.multiLevelSalesParticipantsService.grouping;
    this.paginator = this.multiLevelSalesParticipantsService.paginator;
    this.sorting = this.multiLevelSalesParticipantsService.sorting;
    const sb = this.multiLevelSalesParticipantsService.isLoading$.subscribe((res: any) => this.isLoading = res);
    this.subscriptions.push(sb);
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  filterForm() {
    this.filterGroup = this.fb.group({
      MinDate: [this.MinDate],
      MaxDate: [this.MaxDate],
    });
    this.subscriptions.push(
      this.filterGroup.controls.MinDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.MaxDate.valueChanges.subscribe(() =>
        this.filter()
      )
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const MinDate = this.filterGroup.controls['MinDate'].value;
    const MaxDate = this.filterGroup.controls['MaxDate'].value;
    if (MinDate) {
      filter['MinDate'] = MinDate;
    }
    if (MaxDate) {
      filter['MaxDate'] = MaxDate;
    }
    this.filterValue = filter;
    this.multiLevelSalesParticipantsService.patchState({ filter });
  }

  paginate(paginator: PaginatorState) {
    this.multiLevelSalesParticipantsService.patchState({ paginator });
  }
  
  exportFile() {
    const now = new Date();
    const timeString = now.toLocaleString('en-US', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).replace(/\D/g, '');
    const fileName = `Danhsachnguoithamgiahoatdongbanhangdacap_${timeString}.xlsx`;

    Swal.fire({
            icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })
    const query = {
      filter: this.filterValue,
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: ''
    }

    this.http.post(`${environment.apiUrl}/MultiLevelSalesParticipants/export`, query, {
      responseType: 'blob',
    }).pipe(
      catchError((errorMessage: any) => {
        console.error(errorMessage)
        Swal.fire({
          icon: 'error',
          title: 'Xuất File thất bại',
          confirmButtonText: 'Xác nhận',
        });
        return of();
      }),
    ).subscribe(
      (res) => {
        const file = new Blob([res], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const fileURL = URL.createObjectURL(file);
        const a = document.createElement('a');
        a.href = fileURL;
        a.download = fileName;
        document.body.append(a);
        a.click();
        a.remove();
        URL.revokeObjectURL(fileURL);
        console.log(`File ${fileName} đã tải xong.`);
        Swal.fire({
          icon: 'success',
          title: 'Xuất File thành công',
          confirmButtonText: 'Xác nhận',
        });
      },
    );
  }
}
