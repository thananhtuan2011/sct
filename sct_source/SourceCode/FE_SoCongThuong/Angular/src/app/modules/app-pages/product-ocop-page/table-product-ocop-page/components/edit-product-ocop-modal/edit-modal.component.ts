import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription, filter } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ProductOcopModel } from '../../../_models/product-ocop.model';
import { ProductOcopPageService } from '../../../_services/product-ocop-page.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import { Options } from 'select2';

const EMPTY_CUSTOM: ProductOcopModel = {
  id: '',
  productOcopid: '00000000-0000-0000-0000-000000000000',
  productName: '',
  productOwner: '',
  phoneNumber: '',
  districtId: '00000000-0000-0000-0000-000000000000',
  address: '', //Địa chỉ
  ingredients: '', //Thành phần
  expiry: null, //Hạn sử dụng
  preserve: '', //Bảo quản
  approvalDecision: '', //Quyết định phê duyệt
  ratings: 0, //Số sao đánh giá
};

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],

})
export class EditProductOcopModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  productOcopData: ProductOcopModel;
  formGroup: FormGroup;
  file_images: File[] = [];
  file_images_obj: any[] = [];
  file_documents: any[] = [];
  del_file_ids: string = "";
  districtData: any[] = [];

  private subscriptions: Subscription[] = [];
  options: Options;
  

  constructor(
    private productOcopService: ProductOcopPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.productOcopService.isLoading$;
    this.loadDistrict();
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

  loadDistrict() {
    this.commonService.getDistrict().subscribe((res: any) => {
      const data = [
        { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
        ...res.items.map((item: any) => ({
          id: item.districtId,
          text: item.districtName,
        }))
      ]
      this.districtData = data;
      this.loadProductOcop();
    })
  }

  loadProductOcop() {
    if (!this.id) {
      this.clear();
      this.loadForm();
    } else {
      const sb = this.productOcopService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe(async (res: any) => {
        this.productOcopData = res.data;
        this.loadForm();

        const details = res.data.details
        const image_uploaded = details.filter((x: any) => x.type == 0)

        this.file_documents = details.filter((x: any) => x.type == 1)

        if (image_uploaded.length > 0) {
          for (var image of image_uploaded) {
            let response = await fetch(image.linkFile);
            let data = await response.blob();
            let metadata = { type: 'image/jpeg' };
            const file_image = new File([data], image.linkFile.split('/')[image.linkFile.split('/').length - 1], metadata);
            this.file_images.push(file_image);

            let obj = {
              id: image.productOcopattachFileId,
              file: file_image,
            }
            this.file_images_obj.push(obj);
          }
        }
        if (this.type) {
          this.formGroup.disable();
          this.formGroup.updateValueAndValidity();
        }
      });
      this.subscriptions.push(sb);
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ProductName: [this.productOcopData.productName, Validators.required],
      ProductOwner: [this.productOcopData.productOwner, Validators.required],
      PhoneNumber: [this.productOcopData.phoneNumber, [Validators.minLength(10), Validators.maxLength(11), Validators.pattern('^[0-9]*$')]],
      DistrictId: [this.productOcopData.districtId],
      Address: [this.productOcopData.address, Validators.required],
      Ingredients: [this.productOcopData.ingredients, Validators.required],
      Expiry: [this.productOcopData.expiry, [Validators.required, Validators.pattern('^[0-9]*$')]],
      Preserve: [this.productOcopData.preserve, Validators.required],
      ApprovalDecision: [this.productOcopData.approvalDecision],
      Ratings: [this.productOcopData.ratings],
    });
  }

  clear() {
    EMPTY_CUSTOM.productOcopid = '00000000-0000-0000-0000-000000000000',
    EMPTY_CUSTOM.productName = '',
    EMPTY_CUSTOM.productOwner = '',
    EMPTY_CUSTOM.phoneNumber = '', //Số điện thoại
    EMPTY_CUSTOM.districtId = '00000000-0000-0000-0000-000000000000' //Huyện
    EMPTY_CUSTOM.address = '', //Địa chỉ
    EMPTY_CUSTOM.ingredients = '', //Thành phần
    EMPTY_CUSTOM.expiry = null, //Hạn sử dụng
    EMPTY_CUSTOM.preserve = '', //Bảo quản
    EMPTY_CUSTOM.approvalDecision = '', //Quyết định phê duyệt
    EMPTY_CUSTOM.ratings = 0, //Số sao đánh giá
    this.productOcopData = EMPTY_CUSTOM;
  }

  private prepareProductOcop() {
    var formValue = this.formGroup.value;
    var formData: any = new FormData();

    formData.append('ProductName', formValue.ProductName);
    formData.append('ProductOwner', formValue.ProductOwner);
    formData.append('PhoneNumber', formValue.PhoneNumber);
    formData.append('Address', formValue.Address);
    formData.append('Ingredients', formValue.Ingredients);
    formData.append('Expiry', formValue.Expiry);
    formData.append('Preserve', formValue.Preserve);
    formData.append('ApprovalDecision', formValue.ApprovalDecision);
    formData.append('Ratings', formValue.Ratings);
    formData.append('DistrictId', formValue.DistrictId);

    //File Images
    if (this.file_images.length > 0) {
      let i = 1;
      for (var image of this.file_images_obj) {
        if (image.id == "") {
          formData.append("HinhAnh" + i, image.file, image.file.name);
          i++;
        }
      }
    }

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
    const model = this.prepareProductOcop();
    if (this.id) {
      model.append("ProductOcopid", this.id)
      this.edit(model);
    } else {
      this.create(model);
    }
  }

  edit(item: any) {
    const sbUpdate = this.productOcopService.updateformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.productOcopData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.productOcopData = EMPTY_CUSTOM
    });
    this.subscriptions.push(sbUpdate);
  }

  create(item: any) {
    const sbCreate = this.productOcopService.createformdata(item).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.productOcopData);
      }),
    ).subscribe((res: any) => {
      Swal.fire({
        icon: res.status == 1 ? 'success' : 'error',
        title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
        confirmButtonText: 'Xác nhận',
        text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
      });
      this.productOcopData = EMPTY_CUSTOM
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
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }

  //Images
  onSelectImage(event: any) {
    // console.log(event)
    // console.log(...event.addedFiles)
    for (var image of event.addedFiles) {
      this.file_images.push(image);
      let obj = {
        id: '',
        file: image,
      }
      this.file_images_obj.push(obj);
    }
  }

  onRemoveImage(event: any, index: any) {
    this.del_file_ids += this.file_images_obj[index].id == '' ? '' : this.file_images_obj[index].id + ','
    this.file_images_obj.splice(this.file_images_obj[index], 1);
    this.file_images.splice(this.file_images.indexOf(event), 1);
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
    if (this.file_documents[index].productOcopattachFileId) {
      this.del_file_ids += this.file_documents[index].productOcopattachFileId + ','
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