// import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
// import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
// import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
// import { of, Subscription } from 'rxjs';
// import { catchError, finalize, first, tap } from 'rxjs/operators';
// import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
// import Swal from 'sweetalert2';
// import { Options } from 'select2';
// import { CommonService } from 'src/app/_metronic/shared/services/common.service';
// import * as moment from 'moment';
// import { TradePromotionOtherModel } from '../../../_models/trade-promotion-other.model';
// import { TradePromotionOtherPageService } from '../../../_services/trade-promotion-other-page.service';
// import { environment } from 'src/environments/environment';

// const EMPTY_CUSTOM: TradePromotionOtherModel = {
//   id: '',
//   tradePromotionOtherId: '',
//   tradePromotionOtherContent: '',
//   tradePromotionOtherTime: null,
//   address: '',
//   funding: 0,
//   details: [],
// };
// @Component({
//   selector: 'app-info-trade-promotion-other-modal.component',
//   templateUrl: './info-trade-promotion-other-modal.component.html',
//   styleUrls: ['./info-trade-promotion-other-modal.component.scss'],

// })
// export class InfoTradePromotionOtherModalComponent implements OnInit, OnDestroy {
//   @Input() id: any;
//   isLoading$:any;
//   tradePromotionOtherData: TradePromotionOtherModel;
//   formGroup: FormGroup;
//   public options: Options;
//   dataSource: any[] = [];
//   displayedColumns: string[] = ['stt','name', 'action'];
//   public datKyBaoCao: Array<SelectOptionData>;
//   files: any[] = [];
//   private subscriptions: Subscription[] = [];
//   public default_value = "00000000-0000-0000-0000-000000000000"
//   public data_busi: Array<SelectOptionData>;
//   ListFileDinhKemDel:any='';
//   filedemo: any[]  = [];
//   constructor(
//     private tradePromotionOtherPageService: TradePromotionOtherPageService,
//     private fb: FormBuilder, public modal: NgbActiveModal,
//     private changeDetectorRefs: ChangeDetectorRef,
//     private commonService: CommonService,
//     ) { }

//   ngOnInit(): void {
//     this.isLoading$ = this.tradePromotionOtherPageService.isLoading$;
//     (async () => { 
//     this.loadDetail();
//   })();
//     this.options = {
//       theme: 'bootstrap5',
//       templateSelection: this.templateSelection,
//     };
//   }
//   public templateSelection = (state: any): JQuery | string => {
//     if (!state.id) {
//       return state.text;
//     }
//     return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
//   }

//   loadDetail() {
    
//     if (!this.id) {
//       this.clear_model();
//       this.loadForm();
//     } else {
//       const sb = this.tradePromotionOtherPageService.getItemById(this.id).pipe(
//         first(),
//         catchError((errorMessage) => {
//           this.modal.dismiss(errorMessage);
//           return of(EMPTY_CUSTOM);
//         })
//       ).subscribe((res: any) => {
//         this.tradePromotionOtherData = res.data;
//         this.tradePromotionOtherData.tradePromotionOtherTime=this.convert_date_string(res.data.tradePromotionOtherTime);
//         let detail:{
//           tradePromotionOtherAttachFileId :string;
//           linkFile : string;
//           name : string;
//         };
//         res.data.details.forEach((x:any) => {
//           detail={
//             tradePromotionOtherAttachFileId:x.tradePromotionOtherAttachFileId,
//             linkFile:x.linkFile,
//             name:x.linkFile.split('/sct/FileAll/')[1]
//           }
//           this.files.push(detail);
//         });

//         this.loadForm();

//       });
//       this.subscriptions.push(sb);
//     }
//   }

//   loadForm() {
//     this.formGroup = this.fb.group({
//       tradePromotionOtherContent: [this.tradePromotionOtherData.tradePromotionOtherContent, Validators.required],
//       tradePromotionOtherTime: [this.tradePromotionOtherData.tradePromotionOtherTime, Validators.required],
//       address: [this.tradePromotionOtherData.address, Validators.required],
//       funding: [this.tradePromotionOtherData.funding, Validators.required],
//     });
//     this.formGroup.disable();
//   }
// clear_model() {
//     EMPTY_CUSTOM.tradePromotionOtherId = '',   
//     EMPTY_CUSTOM.tradePromotionOtherContent = '',
//     EMPTY_CUSTOM.tradePromotionOtherTime = null,
//     EMPTY_CUSTOM.address = '',
//     EMPTY_CUSTOM.funding =0,
    
//     EMPTY_CUSTOM.details = [],
//     this.tradePromotionOtherData = EMPTY_CUSTOM
//   }
//   save() {
//     const model = this.prepareData();
//     if (this.tradePromotionOtherData.tradePromotionOtherId!= '') {
//       this.edit(model);
//     } else {
//       this.tradePromotionOtherData.tradePromotionOtherId = this.default_value
//       this.create(model);
//     }
//   }

//   edit(item:any) {
//     const sbUpdate = this.tradePromotionOtherPageService.updateformdata(item,this.tradePromotionOtherData.tradePromotionOtherId).pipe(
//       tap(() => {
//         this.modal.close();
//       }),
//       catchError((errorMessage) => {
//         this.modal.dismiss(errorMessage);
//         return of(this.tradePromotionOtherData);
//       }),
//     ).subscribe((res: any) => {
//       Swal.fire({
//         icon: res.status == 1 ? 'success' : 'error',
//         title: res.status == 1 ? 'Chỉnh sửa thành công' : 'Chỉnh sửa thất bại',
//         confirmButtonText: 'Xác nhận',
//         text: 'Chỉnh sửa ' + (res.status == 1 ? 'thành công' : 'thất bại'),
//       });
//       this.tradePromotionOtherData = EMPTY_CUSTOM
//     });
//     this.subscriptions.push(sbUpdate);
//   }

//   create(item:any) {
//     const sbCreate = this.tradePromotionOtherPageService.createformdata(item).pipe(
//       tap(() => {
//         this.modal.close();
//       }),
//       catchError((errorMessage) => {
//         this.modal.dismiss(errorMessage);
//         return of(this.tradePromotionOtherData);
//       }),
//     ).subscribe((res: any) => {
//       Swal.fire({
//         icon: res.status == 1 ? 'success' : 'error',
//         title: res.status == 1 ? 'Thêm mới thành công' : 'Thêm mới thất bại',
//         confirmButtonText: 'Xác nhận',
//         text: 'Thêm mới ' + (res.status == 1 ? 'thành công' : 'thất bại'),
//       });
//       this.tradePromotionOtherData = EMPTY_CUSTOM
//     });
//     this.subscriptions.push(sbCreate);
//     EMPTY_CUSTOM.tradePromotionOtherId='';
//     this.tradePromotionOtherData = EMPTY_CUSTOM;
//   }
//   convert_date(string_date: string){
//     var result = moment.utc(string_date, "DD/MM/YYYY");
//     return result
//   }

//   convert_date_string(string_date: string){
//     var date = string_date.split("T")[0];
//     var list = date.split("-"); //["year", "month", "day"]
//     var result = list[2] + "/" + list[1] + "/" + list[0]
//     return result
//   }
//   private prepareData() {
    
//     const controls = this.formGroup.value;
//     var formData: any = new FormData();
//     if( this.tradePromotionOtherData.tradePromotionOtherId!="")
//     {
//       formData.append("TradePromotionOtherId", this.tradePromotionOtherData.tradePromotionOtherId);
//     }
//     formData.append("TradePromotionOtherContent", controls["tradePromotionOtherContent"]);
//     formData.append("Address", controls["address"]);
//     formData.append("Funding", +controls["funding"]);
//     formData.append("TradePromotionOtherTimeDisplay",(controls["tradePromotionOtherTime"]) );
//     if (this.filedemo.length > 0) {
//       for (var i = 0; i < this.filedemo[0].length; i++) {
//         if (this.filedemo[0]) {
//           formData.append("Files" + i, this.filedemo[0][i]);
//         }
//       }
//     }
//     if(this.ListFileDinhKemDel!='')
//     {
//       formData.append("IdFiles", this.ListFileDinhKemDel);
//     }
//     // if (this.ListFileDinhKem && this.ListFileDinhKem.length > 0) {
//     //   var strOldFileDinhKem = '';
//     //   this.ListFileDinhKem.forEach(element => {
//     //     if (strOldFileDinhKem == '') {
//     //       strOldFileDinhKem += element.name;
//     //     }
//     //     else {
//     //       strOldFileDinhKem += ","
//     //       strOldFileDinhKem += element.name;
//     //     }
//     //   });
//     //   formData.append("StrListFileDinhKems", strOldFileDinhKem);
//     // }
//     return formData; 
//   }
//   loadProjectById($event:any) {
//     const value=this.dataSource.findIndex(x => x.businessId ==$event);
//     var list:any;
//     list=this.dataSource[value];
//     this.formGroup.controls["address"].setValue(list.diaChi);
//     this.formGroup.updateValueAndValidity();
//   }
// loadBusi() {
//     this.commonService.getBusiness().subscribe((res:any) => {
//       this.dataSource=res.items;
//       const data_busi = [{
//         id: "00000000-0000-0000-0000-000000000000",
//         text: '-- Chọn --'
//       }]
//       for (var item of res.items) {
//         let obj_item = {
//           id: item.businessId,
//           text: item.businessNameVi,
//         }
//         data_busi.push(obj_item)
//       }
//       this.data_busi = data_busi;
//     })
//     return this.data_busi;
//   }

//   isDefaultValue(controlName: any)//: boolean 
//   {
//     const control = this.formGroup.controls[controlName];
//     const isdefaultvalue = (control.value == "00000000-0000-0000-0000-000000000000")
//     if (isdefaultvalue){
//       control.setErrors({default: true})
//     }
//     return control.invalid && (control.dirty || control.touched)
//   }

//   ngOnDestroy(): void {
//     this.subscriptions.forEach(sb => sb.unsubscribe());
//   }

//   // helpers for View
//   isControlValid(controlName: any): boolean {
//     const control = this.formGroup.controls[controlName];
//     return control.valid && (control.dirty || control.touched);
//   }

//   isControlInvalid(controlName: any): boolean {
//     const control = this.formGroup.controls[controlName];
//     return control.invalid && (control.dirty || control.touched);
//   }

//   controlHasError(validation: any, controlName: any): boolean {
//     const control = this.formGroup.controls[controlName];
//     return control.hasError(validation) && (control.dirty || control.touched);
//   }

//   isControlTouched(controlName: any): boolean {
//     const control = this.formGroup.controls[controlName];
//     return control.dirty || control.touched;
//   }
//   prenventInputNonNumber(event: any) {
//     if (event.which < 48 || event.which > 57) {
//       event.preventDefault();
//     }
//   }

//   //Upload File
//   /**
//    * on file drop handler
//    */
//   onFileDropped($event: any) {
//     this.prepareFilesList($event);
//     // this.filedemo($event);
//   }

//   /**
//    * handle file from browsing
//    */
//   fileBrowseHandler(files: any) {
//     this.prepareFilesList(files.target.files);
//     this.filedemo.push(files.target.files);
//   }

//   /**
//    * Delete file from files list
//    * @param index (File index)
//    */
//   deleteFile(item:any,index: number) {
    
//     this.files.splice(index, 1);
//     if (this.ListFileDinhKemDel == '') 
//     {
//       this.ListFileDinhKemDel+=item.tradePromotionOtherAttachFileId;
//     }
//     else 
//     {
//       this.ListFileDinhKemDel += ",";
//       this.ListFileDinhKemDel+=item.tradePromotionOtherAttachFileId;
//     }
//   }

//   /**
//    * Simulate the upload process
//    */
//   uploadFilesSimulator(index: number) {
//     setTimeout(() => {
//       if (index === this.files.length) {
//         return;
//       } else {
//         const progressInterval = setInterval(() => {
//           if (this.files.length === 0) {
//             clearInterval(progressInterval);
//           }
//           else {
//             if (this.files[index].progress === 100) {
//               clearInterval(progressInterval);
//               this.uploadFilesSimulator(index + 1);
//             }
//             else {
//               this.files[index].progress += 5;
//             }
//           }
//         }, 200);
//       }
//     }, 200);
//   }

//   /**
//    * Convert Files list to normal array list
//    * @param files (Files List)
//    */
//   prepareFilesList(files: Array<any>) {
//     for (const item of files) {
//       item.progress = 0;
//       this.files.push(item);
//     }
//     this.uploadFilesSimulator(0);
//   }

//   /**
//    * format bytes
//    * @param bytes (File size in bytes)
//    * @param decimals (Decimals point)
//    */
//   formatBytes(bytes: any, decimals: any) {
//     if (bytes === 0) {
//       return '0 Bytes';
//     }
//     const k = 1024;
//     const dm = decimals <= 0 ? 0 : decimals || 2;
//     const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
//     const i = Math.floor(Math.log(bytes) / Math.log(k));
//     return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
//   }
// }
