import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbDateAdapter, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { EMPTY, of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import { Options } from 'select2';
import Swal from 'sweetalert2';

import {IndustrialManagementTargetModel } from '../../../_models/indus-manage-target.model';
import { IndustrialManagementTargetPageService } from '../../../_services/indus-manage-target.service';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

const EMPTY_CUSTOM : IndustrialManagementTargetModel = {
  id : '',
  industrialManagementTargetId: '00000000-0000-0000-0000-000000000000',
  parentTargetId: '00000000-0000-0000-0000-000000000000',
  groupTargetId: '00000000-0000-0000-0000-000000000000',
  name: '',
  unit: '',
  listChild: [],
}

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})


export class EditIndustrialManagementTargetModalComponent implements OnInit, OnDestroy {
  @Input() id: any;
  @Input() typeCode: any;
  @Input() title: any;
  isLoading$: any;

  formGroup: FormGroup;
  options: Options;
  industrialManagementTargetData: IndustrialManagementTargetModel = {
    id : '',
    industrialManagementTargetId: '00000000-0000-0000-0000-000000000000',
    parentTargetId: '00000000-0000-0000-0000-000000000000',
    groupTargetId: '00000000-0000-0000-0000-000000000000',
    name: '',
    unit: '',
    listChild: [],
  };
  ListConfig: any = [];
  listChildData: any;
  show: boolean = false;
  showGroup: boolean = false;
  QLCNTargetData: any = [];
  groupTargetData : any = [];
  private subscriptions: Subscription[] = [];

  constructor(
    private pageService: IndustrialManagementTargetPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.pageService.isLoading$;
    this.loadQLCNTarget();
    this.loadGroupTarget();
    this.loadData();

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

  loadData() {
    if(!this.id){
      this.clearModel();
      this.loadForm();
    }else{
      const sb = this.pageService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        const data = res.data;
        if(data){
          this.industrialManagementTargetData.parentTargetId = data.parentTargetId;
          this.industrialManagementTargetData.groupTargetId = data.groupTargetId;
          this.industrialManagementTargetData.name = data.name;
          this.industrialManagementTargetData.unit = data.unit;
          this.industrialManagementTargetData.listChild = [{
            Name: data.name,
            Unit: data.unit,
            getChild: data.getChild
          }]
          this.listChildData = data.getChild;
          this.loadForm();
          if(this.listChildData.length > 0){
            for(let i = 0 ; i < this.listChildData.length; i++)
            {
              this.getChild(0).push(this.newChild(this.listChildData[i].name,this.listChildData[i].unit));
            }
          }
        }
      });
      this.subscriptions.push(sb);
     // this.loadForm();
    }
  }

  loadForm() {
    this.formGroup = this.fb.group({
      ParentTargetId: [this.industrialManagementTargetData.parentTargetId],
      GroupTargetId: [this.industrialManagementTargetData.groupTargetId],
      target: this.fb.array([this.fb.group({        
        Name: [this.industrialManagementTargetData.name, Validators.required],
        Unit: [this.industrialManagementTargetData.unit, Validators.required],
        getChild: this.fb.array([])
      })])
    });
    
    
    
    this.checkShowGroupTarget();
    
    this.subscriptions.push(
      this.formGroup.controls.ParentTargetId.valueChanges.subscribe((x) => {
        this.checkShowGroupTarget();
      })
    );
    this.show = true;
  }
  checkShowGroupTarget(){
    const parent = this.QLCNTargetData.find(
      (x: any) => x.id === this.formGroup.controls.ParentTargetId.value
    );
    if(parent && (parent.code === 'QLCN_TARGET_CCN3' || parent.code === 'QLCN_TARGET_CCN6')){
      this.showGroup = true;
    }else{
      this.showGroup = false;
      this.formGroup.controls['GroupTargetId'].reset('00000000-0000-0000-0000-000000000000')
    }
  }
  //Thao tác với FormArray
  // nhận FormArray
  
  // get GetConfig(): FormArray {
  //   return this.formGroup.controls.ListConfig as FormArray;
  // }
  
  target(): FormArray {
    return this.formGroup.get('target') as FormArray;
  } 
  
  getChild(index: number): FormArray {
    return this.target().at(index).get('getChild') as FormArray;
  }
  
  delTarget(index: number){
    this.target().removeAt(index);
  }
  
  addChild(index: number) {
    this.getChild(index).push(this.newChild("",""));
  }
  
  newChild(name: string, unit: string): FormGroup {
    return this.fb.group({
      Name: [name, Validators.required],
      Unit: [unit, Validators.required],
    });
  }
  

  addTarget() {
    this.target().push(this.fb.group({
      Name: ['', Validators.required],
      Unit: ['', Validators.required],
      getChild: this.fb.array([])
    }));
  }
  
  delChild(targetIndex: number, childIndex: number) {
    this.getChild(targetIndex).removeAt(childIndex);
  }

  // // Check Validation
  arrayControlHasError(validation: any, controlName: any, index: any): boolean {
    const control = this.target().controls[index].get(controlName);
    if (control) {
      return control.hasError(validation) && (control.dirty || control.touched);
    }
    else {
      return false
    }
  }
  
  arrayChildControlHasError(validation: any, controlName: any, indexParent: any, index: any): boolean{

   const controlChild = this.target().at(indexParent).get('getChild') as FormArray;
   const control = controlChild.at(index).get(controlName)
    if (control) {
      return control.hasError(validation) && (control.dirty || control.touched);
    }
    else {
      return false
    }
  }

  private prepareData() {
    const formData = this.formGroup.value;
    this.industrialManagementTargetData.parentTargetId = formData.ParentTargetId;
    this.industrialManagementTargetData.groupTargetId = formData.GroupTargetId ? formData.GroupTargetId : '00000000-0000-0000-0000-000000000000';
    this.industrialManagementTargetData.listChild = formData.target;
  }

  save() {
    this.prepareData();
    if (this.id) {
      this.industrialManagementTargetData.industrialManagementTargetId = this.id;
      this.industrialManagementTargetData.id = this.id;
      this.edit();
    } else {
      this.create();
    }
  }

  edit() {
    const sbUpdate = this.pageService.update(this.industrialManagementTargetData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industrialManagementTargetData);
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
    const sbCreate = this.pageService.create(this.industrialManagementTargetData).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(this.industrialManagementTargetData);
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

  check_formGroup() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
    }
    else {
      this.save();
    }
  }
  
  loadQLCNTarget(){
    const sb = this.commonService.GetConfig('QLCN_TARGET').subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --',
          code: ''
        },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName,
          code: item.categoryCode
        }))
      ]
      this.QLCNTargetData = data;
    })
    
    this.subscriptions.push(sb);
  }
  
  loadGroupTarget(){
    const sub = this.commonService.GetConfig('GROUP_TARGET').subscribe((res: any) => {
      const data = [
        {
          id: '00000000-0000-0000-0000-000000000000',
          text: '-- Chọn --'
        },
        ...res.items.listConfig.map((item: any) => ({
          id: item.categoryId,
          text: item.categoryName
        }))
      ]
      this.groupTargetData = data;
    })
    
    this.subscriptions.push(sub);
  }
  
  isDefaultValue(controlName: any): boolean {
    const control = this.formGroup.controls[controlName];
    const value = control.value;
    const isdefaultvalue = (value == "00000000-0000-0000-0000-000000000000")
    if (isdefaultvalue) {
      control.setErrors(
        {
          default_value: true
        }
      )
    }
    return control.invalid && (control.dirty || control.touched);
  }
  
  clearModel(){
    EMPTY_CUSTOM.industrialManagementTargetId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.parentTargetId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.groupTargetId = '00000000-0000-0000-0000-000000000000';
    EMPTY_CUSTOM.name = '';
    EMPTY_CUSTOM.unit = '';
    EMPTY_CUSTOM.listChild = [];
    this.industrialManagementTargetData = EMPTY_CUSTOM;
  }
  
}
