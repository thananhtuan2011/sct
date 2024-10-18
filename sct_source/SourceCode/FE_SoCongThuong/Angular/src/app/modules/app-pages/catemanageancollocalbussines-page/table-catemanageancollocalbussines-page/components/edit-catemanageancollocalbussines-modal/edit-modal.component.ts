import { result } from 'lodash';
import { selectModel } from './../../../../controls-page/controls-custom-page/controls-custom-page.component';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap, throwIfEmpty } from 'rxjs/operators';
import Swal from 'sweetalert2';
import { Options } from 'select2';

import { CateManageAncolLocalBussinesModel } from '../../../_models/catemanageancollocalbussines.model';
import { CateManageAncolLocalBussinesPageService } from '../../../_services/catemanageancollocalbussines-page.service';
import * as moment from 'moment';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM: CateManageAncolLocalBussinesModel = {
  id: '',
  cateManageAncolLocalBussinessId: '00000000-0000-0000-0000-000000000000',
  businessId: '00000000-0000-0000-0000-000000000000', //Tên doanh nghiệp
  numberOfWorker: null,
  investment: null, //Vốn điều lệ
  typeOfProfessionId: '00000000-0000-0000-0000-000000000000', //Nghành nghề kinh doanh chính
  lstProfession: [], //List ngành nghề phụ
  dateRelease: null, //Ngày cấp
  dateChange: null, //Ngày đăng ký thay đổi
  lstWorkers: [], //List danh sách thành viên góp vốn, cổ đông
  isActive: true,
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditCateManageAncolLocalBussinesModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  isLoading$: any;
  catemanageancollocalbussinesData: CateManageAncolLocalBussinesModel;
  formGroup: FormGroup;
  options: Options;
  options_multi: Options;

  businessData: Array<any>;
  typeOfProfessionData: Array<any>;
  typeOfProfessionSelect: Array<any>;
  listWorkersData: Array<any> = [];
  workerName: string;
  listMembersData: Array<any> = [];
  listShareholderData: Array<any> = [];
  listProfessionData: Array<any>;

  private subscriptions: Subscription[] = [];
  show: boolean = false;
  apiLoaded: number = 0;

  constructor(
    private catemanageancollocalbussinesService: CateManageAncolLocalBussinesPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private changeDetectorRefs: ChangeDetectorRef,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.catemanageancollocalbussinesService.isLoading$;
    this.loadBusiness();
    this.loadTypeOfProfession();

    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.options_multi = {
      theme: 'bootstrap5',
      multiple: true,
      // tags: true,
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

  loadBusiness() {
    this.catemanageancollocalbussinesService.LoadBussiness().subscribe((res: any) => {
      var list_business = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
      ]
      for (var item of res.data) {
        let business = {
          id: item.businessId,
          text: item.businessNameVi
        }
        list_business.push(business);
      }
      this.businessData = list_business
      this.loadCateManageAncolLocalBussines();
    })
  }

  selectProfession(event: any) {
    this.typeOfProfessionSelect = this.typeOfProfessionData.filter(x => x.id != event && x.id != '00000000-0000-0000-0000-000000000000')
    this.formGroup.controls['TypeOfProfessionId'].setValue(event);
    this.formGroup.controls['LstProfession'].setValue([]);
    this.formGroup.updateValueAndValidity();
  }

  loadTypeOfProfession() {
    // this.catemanageancollocalbussinesService.LoadTypeOfProfession().subscribe((res: any) => {
    //   var list_profession = [
    //     {
    //       id: '00000000-0000-0000-0000-000000000000',
    //       text: '-- Chọn --',
    //     },
    //   ];
    //   for (var item of res.data) {
    //     let profession = {
    //       id: item.typeOfProfessionId,
    //       text: item.typeOfProfessionName
    //     }
    //     list_profession.push(profession)
    //   }
    //   this.typeOfProfessionData = list_profession
    //   this.typeOfProfessionSelect = list_profession.filter(x => x.id != '00000000-0000-0000-0000-000000000000')
    //   this.loadCateManageAncolLocalBussines();
    // })
    this.commonService.getIndustry().subscribe((res: any) => {
      var list_profession = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
        },
      ];
      for (var item of res.items) {
        let profession = {
          id: item.industryId,
          text: item.industryName
        }
        list_profession.push(profession)
      }
      this.typeOfProfessionData = list_profession
      this.typeOfProfessionSelect = list_profession.filter(x => x.id != '00000000-0000-0000-0000-000000000000')
      this.loadCateManageAncolLocalBussines();
    })
  }

  loadCateManageAncolLocalBussines() {
    if (this.apiLoaded < 1) {
      this.apiLoaded++;
      return
    }
    if (!this.id) {
      this.clear();
      this.loadForm();
      this.show = true;
    } else {
      const sb = this.catemanageancollocalbussinesService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        // console.log(res.data.lstProfession)
        this.listProfessionData = res.data.lstProfession;
        this.catemanageancollocalbussinesData = res.data;
        this.catemanageancollocalbussinesData.isActive = res.data.isActive;
        this.catemanageancollocalbussinesData.lstProfession = this.typeOfProfessionIdEdit(this.listProfessionData);
        this.catemanageancollocalbussinesData.lstWorkers = this.lstWorkersEdit(res.data.lstWorkers);
        this.catemanageancollocalbussinesData.dateRelease = this.convert_date_string(res.data.dateRelease);
        this.catemanageancollocalbussinesData.dateChange = res.data.dateChange != null ? this.convert_date_string(res.data.dateChange) : null;
        this.loadForm();
        this.formGroup.controls.LstWorkers.setValue(this.listWorkersData);
        this.formGroup.controls.Investment.setValue(this.f_currency(this.formGroup.controls.Investment.value));
        this.formGroup.controls.NumberOfWorker.setValue(this.f_currency(this.formGroup.controls.NumberOfWorker.value));
        this.formGroup.updateValueAndValidity();
        this.show = true;
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.catemanageancollocalbussinesData.businessId], //Tên doanh nghiệp
      NumberOfWorker: [this.catemanageancollocalbussinesData.numberOfWorker, Validators.required],
      Investment: [this.catemanageancollocalbussinesData.investment, Validators.required], //Vốn điều lệ
      TypeOfProfessionId: [this.catemanageancollocalbussinesData.typeOfProfessionId], //Nghành nghề kinh doanh chính
      LstProfession: [this.catemanageancollocalbussinesData.lstProfession], //List ngành nghề phụ
      DateRelease: [this.catemanageancollocalbussinesData.dateRelease, Validators.required], //Ngày cấp
      DateChange: [this.catemanageancollocalbussinesData.dateChange], //Ngày đăng ký thay đổi
      LstWorkers: [this.catemanageancollocalbussinesData.lstWorkers], //List danh sách thành viên góp vốn, cổ đông
      IsActive: [this.catemanageancollocalbussinesData.isActive],
      WorkerName: [this.workerName],
    });
    this.subscriptions.push(
      this.formGroup.controls.Investment.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "Investment": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
    this.subscriptions.push(
      this.formGroup.controls.NumberOfWorker.valueChanges.subscribe((x) => {
        this.formGroup.patchValue({
          "NumberOfWorker": this.f_currency(x)
        }, { emitEvent: false })
      })
    );
  }

  clear() {
    EMPTY_CUSTOM.cateManageAncolLocalBussinessId = '00000000-0000-0000-0000-000000000000',
      EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000', //Tên doanh nghiệp
      EMPTY_CUSTOM.numberOfWorker = null,
      EMPTY_CUSTOM.investment = null, //Vốn điều lệ
      EMPTY_CUSTOM.typeOfProfessionId = '00000000-0000-0000-0000-000000000000', //Nghành nghề kinh doanh chính
      EMPTY_CUSTOM.lstProfession = [], //List ngành nghề phụ
      EMPTY_CUSTOM.dateRelease = null, //Ngày cấp
      EMPTY_CUSTOM.dateChange = null, //Ngày đăng ký thay đổi
      EMPTY_CUSTOM.lstWorkers = [], //List danh sách thành viên góp vốn, cổ đông
      EMPTY_CUSTOM.isActive = true,
      this.catemanageancollocalbussinesData = EMPTY_CUSTOM
  }

  private prepareCateManageAncolLocalBussines() {
    this.prepare_listworkers();
    this.formGroup.updateValueAndValidity();
    const formData = this.formGroup.value;
    this.catemanageancollocalbussinesData.businessId = formData.BusinessId;
    this.catemanageancollocalbussinesData.numberOfWorker = Number(formData.NumberOfWorker.replaceAll(',', ''));
    this.catemanageancollocalbussinesData.investment = Number(formData.Investment.replaceAll(',', ''));
    this.catemanageancollocalbussinesData.typeOfProfessionId = formData.TypeOfProfessionId;
    this.catemanageancollocalbussinesData.lstProfession = this.prepare_listProfession();
    this.catemanageancollocalbussinesData.dateRelease = this.convert_date(formData.DateRelease);
    this.catemanageancollocalbussinesData.dateChange = this.convert_date(formData.DateChange);
    this.catemanageancollocalbussinesData.lstWorkers = formData.LstWorkers;
    this.catemanageancollocalbussinesData.isActive = formData.IsActive;
  }

  //Date
  convert_date(string_date: string) {
    var result = moment.utc(string_date, "DD/MM/YYYY");
    return result
  }

  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  //Ô số
  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  //listworkers
  add_member() {
    var workerName = "";
    workerName = this.formGroup.value.WorkerName;
    if (workerName) {
      let obj = {
        CateManageAncolLocalBussinesDetailId: null,
        Fullname: workerName,
        Type: 1,
        IsDel: false
      }
      this.listMembersData.push(obj);
      this.formGroup.controls.WorkerName.setValue("");
      this.formGroup.controls.WorkerName.updateValueAndValidity();
    }
  }

  add_shareholder() {
    var workerName = "";
    workerName = this.formGroup.value.WorkerName;
    if (workerName) {
      let obj = {
        CateManageAncolLocalBussinesDetailId: null,
        Fullname: workerName,
        Type: 2,
        IsDel: false
      }
      this.listShareholderData.push(obj);
      this.formGroup.controls.WorkerName.setValue("");
      this.formGroup.controls.WorkerName.updateValueAndValidity();
    }
  }

  del_member(item: any) {
    // const i = this.listMembersData.findIndex(x => x.Fullname == item.Fullname);
    // this.listMembersData[i].IsDel = true;
    this.listMembersData = this.listMembersData.filter(x => x.Fullname != item.Fullname);

    const ii = this.listWorkersData.findIndex(x => x.Fullname == item.Fullname && x.Type == 1)
    if (ii > -1) {
      this.listWorkersData[ii].IsDel = true;
    }
  }

  del_shareholder(item: any) {
    // const i = this.listShareholderData.findIndex(x => x.Fullname == item.Fullname);
    // this.listShareholderData[i].IsDel = true;
    this.listShareholderData = this.listShareholderData.filter(x => x.Fullname != item.Fullname);

    const ii = this.listWorkersData.findIndex(x => x.Fullname != item.Fullname && x.Type == 2);
    if (ii > -1) {
      this.listWorkersData[ii].IsDel = true;
    }
  }

  prepare_listworkers() {
    for (var item of this.listMembersData) {
      if (item.IsDel != true && item.cateManageAncolLocalBussinesDetailId == null) {
        this.listWorkersData.push(item)
      }
    }
    for (var item2 of this.listShareholderData) {
      if (item2.IsDel != true && item2.cateManageAncolLocalBussinesDetailId == null) {
        this.listWorkersData.push(item2)
      }
    }
    this.formGroup.controls.LstWorkers.setValue(Array.from(new Set(this.listWorkersData)))
    this.formGroup.updateValueAndValidity()
    // console.log(this.formGroup.controls.LstWorkers.value)
  }

  lstWorkersEdit(a: any) {
    this.listWorkersData = [];
    this.listMembersData = [];
    this.listShareholderData = [];

    for (var i of a) {
      let obj = {
        Fullname: i.fullname,
        Type: i.type,
        CateManageAncolLocalBussinesDetailId: i.cateManageAncolLocalBussinesDetailId,
        IsDel: i.isDel
      }
      this.listWorkersData.push(obj)
      if (i.type == 1) {
        this.listMembersData.push(obj)
      }
      else {
        this.listShareholderData.push(obj)
      }
    }
    return this.listWorkersData
  }

  //TypeOfProfession
  typeOfProfessionIdEdit(a: any) {
    const result = []
    for (var i of a) {
      if (this.typeOfProfessionData.find(x => x.id == i.typeOfProfessionId)) {
        result.push(i.typeOfProfessionId)
      }
    }
    return result
  }

  prepare_listProfession() {
    if (this.listProfessionData) {
      // Phần tử chung
      const list = this.listProfessionData.filter(({ typeOfProfessionId }) => this.formGroup.value.LstProfession.find((x: any) => x == typeOfProfessionId))
      // console.log(list)
      // console.log(this.listProfessionData.filter(({typeOfProfessionId}) => this.formGroup.value.LstProfession.find((x: any) => x == typeOfProfessionId)))

      // Phần tử xoá
      const del_list = this.listProfessionData.filter(({ typeOfProfessionId }) => !this.formGroup.value.LstProfession.find((x: any) => x == typeOfProfessionId))
      if (del_list.length > 0) {
        for (var d of del_list) {
          let del_obj = this.listProfessionData.filter(x => x == d && x.isDel == false)[0]
          del_obj.isDel = true
          list.push(del_obj)
        }
      }
      // console.log(del_list)
      // console.log(this.listProfessionData.filter(({typeOfProfessionId}) => !this.formGroup.value.LstProfession.find((x: any) => x == typeOfProfessionId)))

      // Phần tử thêm
      const add_list = this.formGroup.value.LstProfession.filter((x: any) => !this.listProfessionData.find(({ typeOfProfessionId }) => x == typeOfProfessionId))
      if (add_list.length > 0) {
        for (var a of add_list) {
          let new_obj = {
            cateManageAncolLocalBussinesTypeProfessionId: null,
            typeOfProfessionId: a,
            typeOfProfessionName: this.typeOfProfessionData.find(x => x.id == a).text == null ? "" : this.typeOfProfessionData.find(x => x.id == a).text,
            isDel: false
          }
          list.push(new_obj)
        }
      }
      // console.log(add_list)
      // console.log(this.formGroup.value.LstProfession.filter((x : any) => !this.listProfessionData.find(({typeOfProfessionId}) => x == typeOfProfessionId)))

      // Trả kết quả
      const result: any = [];
      for (var i of list) {
        let result_obj = {
          CateManageAncolLocalBussinesTypeProfessionId: i.cateManageAncolLocalBussinesTypeProfessionId,
          TypeOfProfessionId: i.typeOfProfessionId,
          TypeOfProfessionName: this.typeOfProfessionData.find(x => x.id == i.typeOfProfessionId) == null ? "" : this.typeOfProfessionData.find(x => x.id == i.typeOfProfessionId).text,
          IsDel: i.isDel,
        }
        result.push(result_obj)
      }
      // console.log(result)
      return result
    }
    else {
      if (this.formGroup.controls['LstProfession'].value) {
        const result: any = [];
        for (var i of this.formGroup.controls['LstProfession'].value) {
          let result_obj = {
            CateManageAncolLocalBussinesTypeProfessionId: '00000000-0000-0000-0000-000000000000',
            TypeOfProfessionId: i,
            TypeOfProfessionName: i == null ? null : this.typeOfProfessionData.find(x => x.id == i).text,
            IsDel: false,
          }
          result.push(result_obj)
        }
        return result
      }
      else {
        return []
      }
    }
  }

  //CURD
  save() {
    this.prepareCateManageAncolLocalBussines();
    if (this.id) {
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.catemanageancollocalbussinesService.update(this.catemanageancollocalbussinesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.catemanageancollocalbussinesData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create() {
    const sbCreate = this.catemanageancollocalbussinesService.create(this.catemanageancollocalbussinesData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.catemanageancollocalbussinesData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.catemanageancollocalbussinesData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: string): boolean {
    const control = this.formGroup.controls[controlName];
    return control.invalid && (control.dirty || control.touched);
  }

  controlHasError(validation: any, controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.hasError(validation) && (control.dirty || control.touched);
  }

  isControlTouched(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.dirty || control.touched;
  }

  isDefaultValue(controlName: any) {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    if (value == '00000000-0000-0000-0000-000000000000' || value == 0 || value == null) {
      control.setErrors({ defaultvalue: true });
    }
    else {
      control.setErrors({ defaultvalue: null });
      control.updateValueAndValidity();
    }
    return control.invalid && (control.dirty || control.touched);
  }

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
}
