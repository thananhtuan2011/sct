import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ChemicalSafetyCertificateModel } from '../../../_models/chemical-safety-certificate.model';
import { ChemicalSafetyCertificatePageService } from '../../../_services/chemical-safety-certificate-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { AddChemicalInfoModalComponent } from '../add-chemical-info-modal/add-modal.component';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { ViewCertificateModalComponent } from '../view-certificate-modal/view-modal.component';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditChemicalSafetyCertificateModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  private subscriptions: Subscription[] = [];
  isLoading$: any;
  options: Options;
  editData: ChemicalSafetyCertificateModel;
  formGroup: FormGroup;
  show: boolean = false;

  businessData: any[];
  listChemicalData: any[] = [];
  canCertificate: boolean = true;
  file_documents: any[] = [];
  del_file_ids: string = "";

  constructor(
    private service: ChemicalSafetyCertificatePageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService,
    private modalService: NgbModal,
    private http: HttpClient,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.service.isLoading$;
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.loadBusiness();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadBusiness() {
    this.commonService.getBusiness().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.businessId,
          text: item.businessNameVi,
          businessCode: item.businessCode,
          address: item.diaChiTruSo,
          phoneNumber: item.soDienThoai,
        }))
      ]
      this.businessData = data;
      this.loadData();
    })
  }

  loadData() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.service.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(this.clear());
        })
      ).subscribe((res: any) => {
        this.file_documents = res.items[0].details;
        this.editData = res.items[0] as ChemicalSafetyCertificateModel;
        this.listChemicalData = res.items[0].listChemical != null ? res.items[0].listChemical : [];
        
        this.loadForm();

        if (this.type) {
          if (this.editData.num != null && this.editData.validTill != null && this.listChemicalData != null) {
            this.canCertificate = false;
          }
          this.formGroup.disable();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  clear() {
    const EmptyModel = {
      id: '',
      chemicalSafetyCertificateId: '00000000-0000-0000-0000-000000000000',
      businessId: '00000000-0000-0000-0000-000000000000',
      address: '',
      phoneNumber: '',
      fax: '',
      businessAddress: '',
      businessCode: '',
      provider: '',
      businessCertificateDate: '',
      num: '',
      licenseDate: '',
      validTill: '',
      listChemical: [],
    } as ChemicalSafetyCertificateModel;
    this.editData = EmptyModel;
    return EmptyModel; 
  }

  loadForm() {
    this.formGroup = this.fb.group({
      BusinessId: [this.editData.businessId],
      Address: [this.editData.address, Validators.required],
      PhoneNumber: [this.editData.phoneNumber, Validators.compose([Validators.required, Validators.minLength(10), Validators.maxLength(11)])],
      Fax: [this.editData.fax],
      BusinessAddress: [this.editData.businessAddress, Validators.required],
      BusinessCode: [this.editData.businessCode, Validators.required],
      Provider: [this.editData.provider, Validators.required],
      BusinessCertificateDate: [this.editData.businessCertificateDate],
      Num: [this.editData.num],
      LicenseDate: [this.editData.licenseDate],
      ValidTill: [this.editData.validTill],
    })

    this.show = true;

    const businessChange = this.formGroup.controls.BusinessId.valueChanges.subscribe((x: any) => {
      if (x != '00000000-0000-0000-0000-000000000000') {
        const data = this.businessData.find(y => y.id == x);
        if (data != null) {
          this.formGroup.controls.Address.setValue(data.address ?? "");
          this.formGroup.controls.Address.markAsTouched();
          this.formGroup.controls.PhoneNumber.setValue(data.phoneNumber ?? "");
          this.formGroup.controls.PhoneNumber.markAsTouched();
          this.formGroup.controls.BusinessCode.setValue(data.businessCode ?? "");
          this.formGroup.controls.BusinessCode.markAsTouched();
        }
      } else {
        this.formGroup.controls.Address.setValue("");
        this.formGroup.controls.Address.markAsTouched();
        this.formGroup.controls.PhoneNumber.setValue("");
        this.formGroup.controls.PhoneNumber.markAsTouched();
        this.formGroup.controls.BusinessCode.setValue("");
        this.formGroup.controls.BusinessCode.markAsTouched();
      }
    });

    this.subscriptions.push(businessChange);
  }

  private prepare() {
    const formValue = this.formGroup.value;
    // this.editData.businessId = formData.BusinessId;
    // this.editData.address = formData.Address;
    // this.editData.phoneNumber = formData.PhoneNumber;
    // this.editData.fax = formData.Fax;
    // this.editData.businessAddress = formData.BusinessAddress;
    // this.editData.businessCode = formData.BusinessCode;
    // this.editData.provider = formData.Provider;
    // this.editData.licenseDate = formData.LicenseDate;
    // this.editData.num = formData.Num;
    // this.editData.validTill = formData.ValidTill;
    // this.editData.listChemical = this.listChemicalData;

    const formData: any = new FormData();
    formData.append('BusinessId', String(formValue.BusinessId));
    formData.append('Address', String(formValue.Address));
    formData.append('PhoneNumber', String(formValue.PhoneNumber));
    formData.append('Fax', String(formValue.Fax));
    formData.append('BusinessAddress', String(formValue.BusinessAddress));
    formData.append('BusinessCode', String(formValue.BusinessCode));
    formData.append('Provider', String(formValue.Provider));
    formData.append('LicenseDate', String(formValue.LicenseDate));
    formData.append('Num', String(formValue.Num));
    formData.append('BusinessCertificateDate', String(formValue.BusinessCertificateDate));
    formData.append('ValidTill', String(formValue.ValidTill));
    formData.append('JsonListChemical', JSON.stringify(this.listChemicalData));

    //File documents
    if (this.file_documents.length > 0) {
      let i = 1;
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("FileQuyetDinh" + i, document, document.name);
          i++;
        }
      }
    }

    if (this.del_file_ids != "") {
      //Id của file cần xoá
      formData.append("IdFiles", this.del_file_ids)
    }
    
    return formData
  }

  save() {
    const model = this.prepare();
    if (this.id) {
      model.append("ChemicalSafetyCertificateId", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.service.updateFormData(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(item);
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

  create(item: any) {
    const sbCreate = this.service.createFormData(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(item);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: res.status == 0 ? res.error.msg : 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.clear();
    });
    this.subscriptions.push(sbCreate);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

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
    if (value == '00000000-0000-0000-0000-000000000000') {
      control.setErrors({ defaultvalue: true });
      return control.invalid && (control.touched || control.dirty);
    }
    else {
      control.setErrors(null);
      return false;
    }
  }

  prenventInputNonNumber(event: any) {
    if (event.which < 48 || event.which > 57) {
      event.preventDefault();
    }
  }

  check_formGroup() {
    this.formGroup.markAllAsTouched();
    if (!this.formGroup.invalid) {
      this.save()
    }
  }

  addProduction() {
    const modalRef = this.modalService.open(AddChemicalInfoModalComponent, { size: '100px' });
    modalRef.result.then(({ ...res }) => res, (res) => {
      if (res) {
        this.addToList(res)
      }
    })
  }

  addToList(data: string) {
    this.listChemicalData.push(data);
  }

  deleteFromList(index: number) {
    this.listChemicalData.splice(index, 1)
  }

  openView(link: any) {
    const modalRef = this.modalService.open(ViewCertificateModalComponent, { size: 'xl' });
    modalRef.componentInstance.link = link;
  }

  exportCertificate() {
    const moment = require("moment");
    const timeString = moment().format("DDMMYYYYHHmmss");
    const fileName = "GCN_DDK_ATHC_" + timeString + ".pdf"

    Swal.fire({
      icon: 'info',
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    this.http.post(`${environment.apiUrl}/ChemicalSafetyCertificate/ExportCertificate/${this.id}`, {},
      {
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
          const file = new Blob([res], { type: 'application/pdf' });
          const fileURL = URL.createObjectURL(file);

          // this.openView(fileURL);
          const a = document.createElement('a');
          a.href = fileURL;
          a.download = fileName;
          document.body.append(a);
          a.click();
          a.remove();
          URL.revokeObjectURL(fileURL);
          Swal.fire({
            icon: 'success',
            title: 'Xuất File thành công',
            confirmButtonText: 'Xác nhận',
          });
        },
      );
  }

  
  //Files
  //Upload File
  @ViewChild('fileDropRef') fileDropRef: any;
  /**
   * on file drop handler
   */
  onFileDropped($event: any) {
    this.prepareFilesList($event);
  }

  /**
   * handle file from browsing
   */
  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  /**
   * Delete file from files list
   * @param index (File index)
   */
  deleteFile(index: number) {
    this.fileDropRef.nativeElement.value = '';
    if (this.file_documents[index].chemicalSafetyCertificateAttachFileId) {
      this.del_file_ids += this.file_documents[index].chemicalSafetyCertificateAttachFileId + ','
      this.file_documents.splice(index, 1);
    }
    else {
      this.file_documents.splice(index, 1);
    }
  }

  /**
   * Simulate the upload process
   */
  // uploadFilesSimulator(index: number) {
  //   setTimeout(() => {
  //     if (index === this.file_documents.length) {
  //       return;
  //     } else {
  //       const progressInterval = setInterval(() => {
  //         if (this.file_documents.length === 0) {
  //           clearInterval(progressInterval);
  //         }
  //         else {
  //           if (this.file_documents[index].progress === 100) {
  //             clearInterval(progressInterval);
  //             this.uploadFilesSimulator(index + 1);
  //           }
  //           else {
  //             this.file_documents[index].progress += 5;
  //           }
  //         }
  //       }, 200);
  //     }
  //   }, 200);
  // }

  /**
   * Convert Files list to normal array list
   * @param files (Files List)
   */
  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      item.progress = 0;
      this.file_documents.push(item);
    }
    // console.log(this.file_documents)
    // this.uploadFilesSimulator(0);
  }

  /**
   * format bytes
   * @param bytes (File size in bytes)
   * @param decimals (Decimals point)
   */
  formatBytes(bytes: any, decimals: any) {
    if (bytes === 0) {
      return '0 Bytes';
    }
    const k = 1024;
    const dm = decimals <= 0 ? 0 : decimals || 2;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
  }
}
