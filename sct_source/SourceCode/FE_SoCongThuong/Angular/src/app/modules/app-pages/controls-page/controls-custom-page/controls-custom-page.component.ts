import { PaginatorState } from './../../../../_metronic/shared/crud-table/models/paginator.model';
import { ISearchView } from './../../../../_metronic/shared/crud-table/models/search.model';
import { IFilterView } from './../../../../_metronic/shared/crud-table/models/filter.model';
import { ISortView, SortState } from './../../../../_metronic/shared/crud-table/models/sort.model';
import { GroupingState, IGroupingView } from './../../../../_metronic/shared/crud-table/models/grouping.model';
import { ICreateAction, IDeleteAction, IDeleteSelectedAction, IEditAction, IFetchSelectedAction, IUpdateStatusForSelectedAction } from './../../../../_metronic/shared/crud-table/models/table.model';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { catchError, finalize, of, Subscription, tap } from 'rxjs';
import { CustomPageService } from '../_services/custom-page.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EditCustomModalComponent } from './components/edit-custom-modal/edit-custom-modal.component';
import Swal from 'sweetalert2';
import { FormControl, Validators } from '@angular/forms';
import { map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ThemePalette } from '@angular/material/core';
import { DateAdapter } from '@angular/material/core';
import { Moment } from 'moment';
import * as moment from 'moment';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';

export interface selectModel {
  name: string;
  value: number;
}
export interface Task {
  name: string;
  completed: boolean;
  color: ThemePalette;
  subtasks?: Task[];
}
@Component({
  selector: 'app-controls-custom-page',
  templateUrl: './controls-custom-page.component.html',
})
export class ControlsCustomPageComponent implements OnInit,
  OnDestroy,
  ICreateAction,
  IEditAction,
  IDeleteAction,
  IDeleteSelectedAction,
  IFetchSelectedAction,
  IUpdateStatusForSelectedAction,
  ISortView,
  IFilterView,
  IGroupingView,
  ISearchView,
  IFilterView {
  paginator: PaginatorState;
  sorting: SortState;
  grouping: GroupingState;
  isLoading: boolean;
  filterGroup: FormGroup;
  searchGroup: FormGroup;
  private subscriptions: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  public exampleData: Array<SelectOptionData>;
  public options: Options;


  // For fill controls
  // Select Control
  selects: selectModel[] = [{ name: 'Select 1', value: 1 },
  { name: 'Select 2', value: 2 },
  { name: 'Select 3', value: 3 },];
  selected = new FormControl(this.selects[0]);
  selectSearch: selectModel[] = [{ name: 'TP.HCM', value: 1 },
  { name: 'Vũng Tàu', value: 2 },
  { name: 'Quảng Ngãi', value: 3 },
  { name: 'Thanh Hóa', value: 4 },
  { name: 'Nghệ An', value: 5 },
  { name: 'Hà Tĩnh', value: 6 },
  { name: 'Bình Định', value: 7 },
  { name: 'Bến Tre', value: 8 },];
  filteredSearch: Observable<selectModel[]>;
  filteredSearch2: Observable<selectModel[]>;
  selectControl = new FormControl(this.selectSearch[0], Validators.required);
  selectControl2 = new FormControl(this.selectSearch[4]);
  searchControl = new FormControl('');
  searchControl2 = new FormControl('');
  selectedDate = new FormControl(new Date());
  dateOfBirth = new FormControl(new Date(), Validators.required);
  dateOfBirth2 = new FormControl(new Date(), Validators.required);
  start = new FormControl('', Validators.required);
  end = new FormControl('', Validators.required);
  // Radio button Control
  radiobuttons: selectModel[] = [
    { name: 'Nam', value: 1 },
    { name: 'Nữ', value: 2 },
  ];
  selectedRadioButton = new FormControl(this.radiobuttons[0]);

  //Check box
  task: Task = {
    name: 'Indeterminate',
    completed: false,
    color: 'primary',
    subtasks: [
      { name: 'Primary', completed: false, color: 'primary' },
      { name: 'Accent', completed: false, color: 'accent' },
      { name: 'Warn', completed: false, color: 'warn' },
    ],
  }; allComplete: boolean = false;

  //Autocomplete control
  // myAutoCompControl = new FormControl('');
  panelOpenState = false;
  minDate: Moment;
  maxDate: Moment;
  result: string;
  constructor(
    private fb: FormBuilder,
    public customPageService: CustomPageService,
    private modalService: NgbModal,
    private _adapter: DateAdapter<any>,//for vietnamese
  ) { }
  ngOnInit(): void {
    this._adapter.setLocale('vi');//For vietnamese
    //Set validate min,max value of date
    const currentYear = moment().year();
    this.minDate = moment([currentYear - 1, 0, 1]);//moment([yyyy,MM(0-11),DD]);
    this.maxDate = moment([currentYear + 1, 11, 29])//MM=0 = tháng 1
    //
    this.filteredSearch = this.searchControl.valueChanges.pipe(
      startWith(''),
      map(value => {
        const name = typeof value === 'string' ? value : value;
        return name ? this._filter(name as string) : this.selectSearch.slice();
      }),
    );
    this.filteredSearch2 = this.searchControl2.valueChanges.pipe(
      startWith(''),
      map(value => {
        const name = typeof value === 'string' ? value : null;
        return name ? this._filter(name as string) : this.selectSearch.slice();
      }),
    );//Khởi tạo search khi thay đổi text search

    // this.filterForm();
    // this.searchForm();
    // this.customPageService.fetch();
    // this.grouping = this.customPageService.grouping;
    // this.paginator = this.customPageService.paginator;
    // this.sorting = this.customPageService.sorting;
    // const sb = this.customPageService.isLoading$.subscribe((res:any) => this.isLoading = res);
    // this.subscriptions.push(sb);

    //new control
    this.exampleData = [
      {
        id: '0',
        text: '-- Chọn --'
      },
      {
        id: 'basic1',
        text: 'Basic 1'
      },
      {
        id: 'basic2',
        text: 'Basic 2'
      },
      {
        id: 'basic3',
        text: 'Basic 3'
      },
      {
        id: 'basic4',
        text: 'Basic 4'
      }
    ];

    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection
    }
  }

  // function for selection template
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }


  displayFn(selectModel: selectModel): string {
    return selectModel && selectModel.name ? selectModel.name : '';
  }
  private _filter(name: string): selectModel[] {
    let a = this.dateOfBirth.value;
    const filterValue = name.toLowerCase();
    return this.selectSearch.filter(option => option.name.toLowerCase().includes(filterValue));
  }
  show(type: string) {
    switch (type) {
      case 'select': {
        this.result = this.selected.value?.name || '';
        break;
      }
      case 'selectSearch': {
        this.result = this.selectControl.value?.name || '';
        break;
      }
      case 'radioButton': {
        this.result = this.selectedRadioButton.value?.name || '';
        break;
      }
      case 'selectSearch': {
        //statements; radioButton
        break;
      }
      default: {
        //statements;
        break;
      }
    }
  }

  //check box
  updateAllComplete() {
    this.allComplete = this.task.subtasks != null && this.task.subtasks.every(t => t.completed);
  }

  someComplete(): boolean {
    if (this.task.subtasks == null) {
      return false;
    }
    return this.task.subtasks.filter(t => t.completed).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.task.subtasks == null) {
      return;
    }
    this.task.subtasks.forEach(t => (t.completed = completed));
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sb) => sb.unsubscribe());
    this.customPageService.setDefaults();
  }

  // filtration
  filterForm() {
    this.filterGroup = this.fb.group({
      status: [''],
      type: [''],
      searchTerm: [''],
    });
    this.subscriptions.push(
      this.filterGroup.controls.status.valueChanges.subscribe(() =>
        this.filter()
      )
    );
    this.subscriptions.push(
      this.filterGroup.controls.type.valueChanges.subscribe(() => this.filter())
    );
  }

  filter() {
    const filter: { [key: string]: string } = {};
    const status = this.filterGroup.controls['status'].value;
    if (status) {
      filter['status'] = status;
    }
    this.customPageService.patchState({ filter });
  }

  // search
  searchForm() {
    this.searchGroup = this.fb.group({
      searchTerm: [''],
    });
    const searchEvent = this.searchGroup.controls.searchTerm.valueChanges
      .subscribe((val) => this.search(val));
    this.subscriptions.push(searchEvent);
  }

  search(searchTerm: string) {
    if (searchTerm.length == 0)
      this.customPageService.patchState({ searchTerm });
  }

  onEnter() {
    this.searchData(this.searchGroup.controls.searchTerm.value)
  }

  searchData(searchTerm: string) {
    this.customPageService.patchState({ searchTerm })
  }

  // sorting
  sort(column: string) {
    const sorting = this.sorting;
    const isActiveColumn = sorting.column === column;
    if (!isActiveColumn) {
      sorting.column = column;
      sorting.direction = 'asc';
    } else {
      sorting.direction = sorting.direction === 'asc' ? 'desc' : 'asc';
    }
    this.customPageService.patchState({ sorting });
  }

  // pagination
  paginate(paginator: PaginatorState) {
    this.customPageService.patchState({ paginator });
  }

  // form actions
  create() {
    this.edit(0);
  }

  edit(id: number) {
    const modalRef = this.modalService.open(EditCustomModalComponent, { size: 'lg' });
    modalRef.componentInstance.id = id;
    modalRef.result.then(() =>
      this.customPageService.fetch(),
      () => { }
    );
  }

  delete(id: number) {
    Swal.fire({
      title: 'Bạn có muốn xóa ?',
      text: 'Hành động này không thể hoàn tác!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        const sb = this.customPageService.delete(id).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
            });
            this.filter()
          }),
          catchError((errorMessage) => {
            return of(undefined);
          }),
          finalize(() => {
            this.isLoading = false;
          })
        ).subscribe();
      }
    })
  }

  deleteSelected() {
    Swal.fire({
      title: 'Bạn có muốn xóa ?',
      text: 'Hành động này không thể hoàn tác!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Thoát'
    }).then((result) => {
      if (result.isConfirmed) {
        this.isLoading = true;
        const sb = this.customPageService.deleteItems(this.grouping.getSelectedRows()).pipe(
          tap((res) => {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Thành công' : 'Thất bại',
              confirmButtonText: 'Xác nhận',
              text: 'Xóa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
            });
            this.filter()
          }),
          catchError((errorMessage) => {
            return of(undefined);
          }),
          finalize(() => {
            this.isLoading = false;
          })
        ).subscribe();
      }
    })
  }

  updateStatusForSelected() {
  }

  fetchSelected() {
  }
}


