import { CommonService } from './../../../../../../_metronic/shared/services/common.service';
import { ChangeDetectorRef, Component, Injectable, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';


import { BusinessModel } from '../../../_models/business.model';
import { BusinessPageService } from '../../../_services/business-page.service';

import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { Options } from 'select2';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';
import * as moment from 'moment';


//Model business
const EMPTY_CUSTOM: BusinessModel = {
  id: '',
  businessId: '00000000-0000-0000-0000-000000000000',


  //Thông tin doanh nghiệp
  businessCode: '',
  tenGiaoDich: '',
  businessNameEn: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  communeId: '00000000-0000-0000-0000-000000000000',
  diaChiTruSo: '',
  ngayCapPhep: null,
  maSoThue: '',
  businessNameVi: '',
  loaiHinhDoanhNghiep: '00000000-0000-0000-0000-000000000000', //Id và Name
  loaiNganhNghe: '00000000-0000-0000-0000-000000000000', //Id và Name
  ngayHoatDong: null,
  giayPhepSanXuat: '',

  //Thông tin liên lạc
  nguoiDaiDien: '',
  soDienThoai: '',
  ngaySinh: '',
  cccd: '',
  ngayCap: '',
  noiCap: '',
  diaChi: '',
  giamDoc: '',
  email: '',

  //Thông tin ngành nghề
  industryId: [],
};

//Model dataSource
export interface selectitem {
  id: string,
  code: string,
  name: string
}

@Component({
  selector: 'app-edit-business-modal',
  templateUrl: './edit-business-modal.component.html',
  styleUrls: ['./edit-business-modal.component.scss'],

})
export class EditBusinessModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  businessData: BusinessModel;
  formGroup: FormGroup;
  apiLoaded: number = 0;
  minDate: any = { day: 1, month: 1, year: 1975 };
  maxDate: any = { day: new Date().getDate(), month: new Date().getMonth() + 1, year: new Date().getFullYear() };

  //Search bar
  filtered: Observable<any[]>;

  //Các cột hiển thị
  displayedColumns: string[] = ['select', 'number', 'code', 'name', 'action'];

  private subscriptions: Subscription[] = [];
  public options: Options;

  //Dữ liệu từ API
  public selectData: Array<SelectOptionData>;
  public typeofbusinessData: Array<SelectOptionData>;
  public typeofprofessionData: Array<SelectOptionData>;

  // 2 Array cho select
  public industryData: any[] = [];
  public industryDataSelect: any[] = [];
  public selectedIndustries: string = '00000000-0000-0000-0000-000000000000';
  public listSelect: string[] = [];

  public communeData: any = [];
  public districtData: any = [];
  public communeByDistrictData: any = [];

  //Selection Items lưu vào đây
  public selection = new SelectionModel<selectitem>(true, []);
  public statusDataSource = false;

  //DataSource cho table
  public dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect)

  //Logo Image
  file_images: File[] = [];
  file_images_obj: any[] = [];
  del_file_ids: string = "";
  TouchImage: boolean = false;
  ChangeImage: boolean = false;
  show: boolean = false;

  constructor(
    private businessService: BusinessPageService,
    private commonService: CommonService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private cd: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.businessService.isLoading$;
    this.loadTypeOfBusiness();
    this.loadTypeOfProfession();
    this.loadIndustry();
    this.loadDistrict();
    this.loadCommune();

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

  //Load Data
  loadIndustry() {
    this.businessService.loadIndustries().subscribe(res_industry => {
      let data_industry = [
        {
          id: "00000000-0000-0000-0000-000000000000",
          text: '-- Chọn ngành nghề --',
          name: "Industry",
          code: "I"
        }
      ]
      for (var industry of res_industry.items) {
        if (industry.industryLevel != 0) {
          let obj_industry = {
            id: industry.industryId,
            text: industry.industryCode + " - " + industry.industryName,
            name: industry.industryName,
            code: industry.industryCode
          }
          data_industry.push(obj_industry)
        }
      }
      this.industryData = data_industry
      this.industryData = this.industryData.sort((i1, i2) => {
        if (i1.text > i2.text) {
          return 1;
        }
        if (i1.text < i2.text) {
          return -1;
        }
        return 0;
      })
      this.loadBusiness();
    })
  }

  loadTypeOfBusiness() {
    this.businessService.loadTypeOfBusinesses().subscribe(res_typeofbusiness => {
      const data_typeofbusiness = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn --'
      }]
      for (var typeofbusiness of res_typeofbusiness.items) {
        let obj_typeofbusiness = {
          id: typeofbusiness.typeOfBusinessId,
          text: typeofbusiness.typeOfBusinessName,
        }
        data_typeofbusiness.push(obj_typeofbusiness)
      }
      this.typeofbusinessData = data_typeofbusiness
      this.loadBusiness();
    })
  }

  loadTypeOfProfession() {
    this.businessService.loadTypeOfProfession().subscribe(res_profession => {
      const data_typeofprofession = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn / không có --'
      }]
      for (var typeofprofession of res_profession.items) {
        let obj_typeofprofession = {
          id: typeofprofession.typeOfProfessionId,
          text: typeofprofession.typeOfProfessionName,
        }
        data_typeofprofession.push(obj_typeofprofession)
      }
      this.typeofprofessionData = data_typeofprofession
      this.loadBusiness();
    })
  }

  loadCommune() {
    this.commonService.getCommune().subscribe((res_commune: any) => {
      const communes = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn Xã --',
        districtId: '',
      }]
      for (var commune of res_commune.items) {
        let obj_commune = {
          id: commune.communeId,
          text: commune.communeName,
          districtId: commune.districtId,
        }
        communes.push(obj_commune)
      }
      this.communeByDistrictData = communes;
      this.communeData = communes.sort((a, b) => {
        if (a < b) {
          return -1;
        }
        if (a > b) {
          return 1;
        }
        return 0;
      });
      this.loadBusiness();
    })
  }

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res_district: any) => {
      const districts = [{
        id: "00000000-0000-0000-0000-000000000000",
        text: '-- Chọn Huyện--',
      }];
      for (var district of res_district.items) {
        let obj_district = {
          id: district.districtId,
          text: district.districtName
        }
        districts.push(obj_district)
      }
      this.districtData = districts.sort((a, b) => {
        if (a.text < b.text) {
          return -1;
        }
        if (a.text > b.text) {
          return 1;
        }
        return 0;
      });
      this.loadBusiness();
    })
  }

  loadBusiness() {
    if (this.apiLoaded < 4) {
      this.apiLoaded++
      return
    }
    if (!this.id) {
      this.clear_model();
      this.loadForm();
      this.show = true;
    } else {
      const sb = this.businessService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe(async (res: any) => {
        this.businessData = res.items[0];

        //Clear 2 Array
        this.industryDataSelect = [];
        this.listSelect = [];
        //Nếu có ngành hàng
        if (this.businessData.industryId.length > 0) {
          for (var item of this.businessData.industryId) {
            var obj = this.industryData.find(x => x.id === item);
            //nếu obj tồn tại thì thêm nó vào Array Select
            if (obj) {
              this.industryDataSelect.push(obj);
              this.listSelect.push(obj.id);
            }
          };

          //Sắp xếp lại ngành hàng
          this.industryData = this.industryData.sort((i1, i2) => {
            if (i1.code > i2.code) {
              return 1;
            }
            if (i1.code < i2.code) {
              return -1;
            }
            return 0;
          });
        }
        this.dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect);
        this.loadForm();
        const image = res.items[0].details[0];
        if (image) {
          let response = await fetch(image.linkFile);
          let data = await response.blob();
          let metadata = { type: 'image/jpeg' };
          const file_image = new File([data], image.linkFile.split('/')[image.linkFile.split('/').length - 1], metadata);
          this.file_images.push(file_image);
          let obj = {
            id: image.logoId,
            file: file_image,
          }
          this.file_images_obj.push(obj);
        }

        if (this.type) {
          this.formGroup.disable();
        }
        this.formGroup.updateValueAndValidity();
        this.check_dataSource()

        this.show = true;
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    let result = this.communeData.filter((x: any) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == this.businessData.districtId);
    this.communeByDistrictData = result;
    this.formGroup = this.fb.group({
      BusinessCode: [this.businessData.businessCode, Validators.required], //Validators.compose([, Validators.pattern("[0-9]{1,15}")])
      TenGiaoDich: [this.businessData.tenGiaoDich, Validators.required], //Validators.compose([, Validators.minLength(10), Validators.maxLength(200)])
      BusinessNameEn: [this.businessData.businessNameEn],
      DistrictId: [this.businessData.districtId, Validators.required],
      CommuneId: [this.businessData.communeId, Validators.required],
      DiaChiTruSo: [this.businessData.diaChiTruSo],
      NgayCapPhep: [this.businessData.ngayCapPhep], //[this.converDateToNbgDate(this.businessData.ngayCapPhep)],
      MaSoThue: [this.businessData.maSoThue],
      BusinessNameVi: [this.businessData.businessNameVi, Validators.required], //Validators.compose([, Validators.minLength(10), Validators.maxLength(200)])
      LoaiHinhDoanhNghiep: [this.businessData.loaiHinhDoanhNghiep],
      LoaiNganhNghe: [this.businessData.loaiNganhNghe],
      NgayHoatDong: [this.businessData.ngayHoatDong], //[this.converDateToNbgDate(this.businessData.ngayHoatDong)]
      NguoiDaiDien: [this.businessData.nguoiDaiDien],
      SoDienThoai: [this.businessData.soDienThoai], //, Validators.compose([Validators.pattern("[0-9]{1,15}")])
      NgaySinh : [this.businessData.ngaySinh],
      CCCD : [this.businessData.cccd, Validators.compose([Validators.minLength(9), Validators.maxLength(12)])],
      NgayCapCCCD : [this.businessData.ngayCap],
      NoiCapCCCD : [this.businessData.noiCap],
      DiaChi: [this.businessData.diaChi],
      GiamDoc: [this.businessData.giamDoc],
      Email: [this.businessData.email],
      IndustryId: [this.businessData.industryId],
      Search: [''],
      Selected: [this.selectedIndustries],
      GiayPhepSanXuat: [this.businessData.giayPhepSanXuat],
    });
    this.subscriptions.push(
      this.formGroup.controls.Selected.valueChanges.subscribe((result) => {
        if (result !== '00000000-0000-0000-0000-000000000000') {
          const index = this.industryData.findIndex(x => x.id === result);
          const check = this.industryDataSelect.findIndex(x => x.id === result);
          if (check === -1) {
            const item = this.industryData[index];
            this.industryDataSelect.push(item)
            this.listSelect.push(result);
            this.dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect);
            this.check_dataSource()
          }
        }
        this.formGroup.patchValue({
          "Selected": '00000000-0000-0000-0000-000000000000'
        }, { emitEvent: false })
      })
    );
  }

  //prenventInputNonNumber
  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  save() {
    const model = this.prepareBusiness();
    if (this.id) {
      model.append("BusinessId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(model: any) {
    Swal.fire({
      icon: 'info',
      title: 'Đang chỉnh sửa, vui lòng đợi trong giây lát.',
      didOpen: () => {
        Swal.showLoading()
      },
      allowOutsideClick: false
    })

    const sbUpdate = this.businessService.updateFormData(model).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.businessData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbUpdate);
  }

  create(model: any) {
    Swal.fire({
      icon: 'info',
      title: 'Đang thêm mới, vui lòng đợi trong giây lát.',
      didOpen: () => {
        Swal.showLoading()
      },
      allowOutsideClick: false
    })

    const sbCreate = this.businessService.createFormData(model).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.businessData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
    });
    this.subscriptions.push(sbCreate);
  }

  private prepareBusiness() {
    const formValue = this.formGroup.value;
    var formData: any = new FormData();

    // Thông tin công ty
    formData.append('BusinessCode', formValue.BusinessCode);
    formData.append('TenGiaoDich', formValue.TenGiaoDich);
    formValue.BusinessNameEn ? formData.append('BusinessNameEn', formValue.BusinessNameEn) : null;
    formValue.DistrictId == '00000000-0000-0000-0000-000000000000' ? null : formData.append('DistrictId', formValue.DistrictId);
    formValue.CommuneId == '00000000-0000-0000-0000-000000000000' ? null : formData.append('CommuneId', formValue.CommuneId);
    formValue.DiaChiTruSo ? formData.append('DiaChiTruSo', formValue.DiaChiTruSo) : null;
    formValue.NgayCapPhep ? formData.append('NgayCapPhep', formValue.NgayCapPhep) : null;
    formValue.MaSoThue ? formData.append('MaSoThue', formValue.MaSoThue) : null;
    formData.append('BusinessNameVi', formValue.BusinessNameVi);
    formData.append('LoaiHinhDoanhNghiep', formValue.LoaiHinhDoanhNghiep);
    formValue.LoaiNganhNghe == '00000000-0000-0000-0000-000000000000' ? null : formData.append('LoaiNganhNghe', formValue.LoaiNganhNghe);
    formValue.NgayHoatDong ? formData.append('NgayHoatDong', formValue.NgayHoatDong) : null;
    formData.append('GiayPhepSanXuat', formValue.GiayPhepSanXuat);
    
    // Thông tin người đại diện
    formValue.NguoiDaiDien ? formData.append('NguoiDaiDien', formValue.NguoiDaiDien) : null;
    formValue.SoDienThoai ? formData.append('SoDienThoai', formValue.SoDienThoai) : null;
    formValue.NgaySinh ? formData.append('NgaySinh', formValue.NgaySinh) : null;
    formValue.CCCD ? formData.append('Cccd', formValue.CCCD) : null;
    formValue.NgayCapCCCD ? formData.append('NgayCap', formValue.NgayCapCCCD) : null;
    formValue.NoiCapCCCD ? formData.append('NoiCap', formValue.NoiCapCCCD) : null;
    formValue.DiaChi ? formData.append('DiaChi', formValue.DiaChi) : null;
    formValue.GiamDoc ? formData.append('GiamDoc', formValue.GiamDoc) : null;
    formValue.Email ? formData.append('Email', formValue.Email) : null;

    // Thông tin ngành hàng
    this.listSelect.length > 0 ? formData.append('IndustryIdString', JSON.stringify(this.listSelect)) : null;
    
    //File Logo Images
    if (this.file_images.length > 0 && this.ChangeImage == true) {
      formData.append("", this.file_images[0], this.file_images[0].name);
    }

    //Xoá Logo
    if (this.del_file_ids != "") {
      //Id của file cần xoá
      formData.append("IdFiles", this.del_file_ids)
    }

    return formData
  }

  clear_model() {
    EMPTY_CUSTOM.businessId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.businessCode = '',
    EMPTY_CUSTOM.tenGiaoDich = '',
    EMPTY_CUSTOM.businessNameEn = '',
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.communeId = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.diaChiTruSo = '',
    EMPTY_CUSTOM.ngayCapPhep = null,
    EMPTY_CUSTOM.maSoThue = '',
    EMPTY_CUSTOM.businessNameVi = '',
    EMPTY_CUSTOM.loaiHinhDoanhNghiep = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.loaiNganhNghe = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.ngayHoatDong = null,
    EMPTY_CUSTOM.nguoiDaiDien = '',
    EMPTY_CUSTOM.soDienThoai = '',
    EMPTY_CUSTOM.ngaySinh = '',
    EMPTY_CUSTOM.cccd = '',
    EMPTY_CUSTOM.ngayCap = '',
    EMPTY_CUSTOM.noiCap = '',
    EMPTY_CUSTOM.diaChi = '',
    EMPTY_CUSTOM.giamDoc = '',
    EMPTY_CUSTOM.email = '',
    EMPTY_CUSTOM.industryId = [],
    EMPTY_CUSTOM.giayPhepSanXuat = '',
    this.businessData = EMPTY_CUSTOM
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  // helpers for View
  isControlValid(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    return control.valid && (control.dirty || control.touched);
  }

  isControlInvalid(controlName: any): boolean {
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

  //Select
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  isTouched(controlName: any) {
    const control = this.formGroup.controls[controlName];
  }

  isDefaultValue(controlName: any)//: boolean 
  {
    const control = this.formGroup.controls[controlName];
    const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors({ default: true })
    }
    return control.invalid && (control.dirty || control.touched)
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  // selectIndustry(item: any) {
  //   this.industryDataSelect.push(item)
  //   this.listSelect.push(item.id)
  //   this.industryData = this.industryData.filter(obj => obj.id !== item.id);
  //   this.industryData = this.industryData.sort((i1, i2) => {
  //     if (i1.code > i2.code) {
  //       return 1;
  //     }
  //     if (i1.code < i2.code) {
  //       return -1;
  //     }
  //     return 0;
  //   });
  //   this.dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect)
  // }

  removeIndustry(item: any) {
    if (this.selection.isSelected(item)) {
      this.selection.deselect(item)
    }
    this.industryData.push(item)
    this.industryDataSelect = this.industryDataSelect.filter(obj => obj.id !== item.id);
    this.listSelect = this.listSelect.filter(obj => obj !== item.id);
    this.industryData = this.industryData.sort((i1, i2) => {
      if (i1.code > i2.code) {
        return 1;
      }
      if (i1.code < i2.code) {
        return -1;
      }
      return 0;
    });
    this.dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect)
    this.check_dataSource()
  }

  removeSelectedRows() {
    this.selection.selected.forEach(item => {
      // this.industryData.push(item)
      // let index: number = this.industryDataSelect.findIndex(d => d === item);
      // this.industryDataSelect.splice(index, 1);
      // this.dataSource = new MatTableDataSource<selectitem>(this.industryDataSelect);
      this.removeIndustry(item);
    });
    // this.industryData = this.industryData.sort((i1, i2) => {
    //   if (i1.code > i2.code) {
    //     return 1;
    //   }
    //   if (i1.code < i2.code) {
    //     return -1;
    //   }
    //   return 0;
    // });
    // this.selection = new SelectionModel<selectitem>(true, []);
    // this.check_dataSource()
  }

  check_dataSource() {
    if (this.dataSource.data.length > 0) {
      this.statusDataSource = true
    } else {
      this.statusDataSource = false
    }
  }

  check_formGroup() {
    //|| this.file_images.length < 1
    if (this.formGroup.invalid) {
      //this.TouchImage = true;
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  //Images
  onSelectImage(event: any) {
    this.TouchImage = true;
    if (this.file_images.length > 0) {
      this.file_images = [];
      this.file_images_obj = [];
      for (var image of event.addedFiles) {
        this.file_images.push(image);
        let obj = {
          id: '',
          file: image,
        }
        this.file_images_obj.push(obj);
      }
    } else {
      for (var image of event.addedFiles) {
        this.file_images.push(image);
        let obj = {
          id: '',
          file: image,
        }
        this.file_images_obj.push(obj);
      }
    }
    this.ChangeImage = true;
  }

  onRemoveImage(event: any, index: any) {
    this.del_file_ids += this.file_images_obj[index].id == '' ? '' : this.file_images_obj[index].id + ','
    this.file_images_obj = [];
    this.file_images = [];
  }

  loadCommuneByDistrict(event: any) {
    if (event != '00000000-0000-0000-0000-000000000000') {
      let result = this.communeData.filter((x: any) => x.id == '00000000-0000-0000-0000-000000000000' || x.districtId == event);
      this.communeByDistrictData = result;
    }
    else {
      this.communeByDistrictData = this.communeByDistrictData;
    }
    this.formGroup.controls['CommuneId'].setValue('00000000-0000-0000-0000-000000000000')
  }
}