import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { ReportAdministrativeProceduresModel } from '../../../_models/report-administrative-procedures.model';
import { ReportAdministrativeProceduresPageService } from '../../../_services/report-administrative-procedures-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';
import * as moment from 'moment';

@Component({
    selector: 'app-edit-modal',
    templateUrl: './edit-modal.component.html',
    styleUrls: ['./edit-modal.component.scss'],
})

export class EditReportAdministrativeProceduresModalComponent implements OnInit, OnDestroy {
    @Input() id: any;
    @Input() type: any;
    private subscriptions: Subscription[] = [];
    isLoading$: any;
    options: Options;
    editData: ReportAdministrativeProceduresModel;
    formGroup: FormGroup;
    show: boolean = false;
    periodData: any[];
    fieldData: any[];
    apiLoaded: number = 0;

    constructor(
        private service: ReportAdministrativeProceduresPageService,
        private fb: FormBuilder,
        public modal: NgbActiveModal,
        private commonService: CommonService,
    ) { }

    ngOnInit(): void {
        this.isLoading$ = this.service.isLoading$;
        this.options = {
            theme: 'bootstrap5',
            templateSelection: this.templateSelection,
        };
        this.loadPeriod();
        this.loadField();
    }

    public templateSelection = (state: any): JQuery | string => {
        if (!state.id) {
            return state.text;
        }
        return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
    }

    loadPeriod() {
        this.commonService.GetConfig('REPORT_ADMINISTRATIVE_PROCEDURES_PERIOD').subscribe((res: any) => {
            const data = [
                { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
                ...res.items.listConfig.map((item: any) => ({
                    id: item.categoryId,
                    text: item.categoryName,
                }))
            ]
            this.periodData = data;
            this.loadData();
        })
    }

    loadField() {
        this.commonService.GetConfig('ADMINISTRATIVE_PROCEDURE_FIELD').subscribe((res: any) => {
            const data = [
                { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
                ...res.items.listConfig.map((item: any) => ({
                    id: item.categoryId,
                    text: item.categoryName,
                }))
            ]
            this.fieldData = data;
            this.loadData();
        })
    }

    loadData() {
        this.apiLoaded++
        if (this.apiLoaded < 2) {
            return
        }
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
                this.editData = res.items[0] as ReportAdministrativeProceduresModel;
                this.loadForm();
                if (this.type) {
                    this.formGroup.disable();
                }
            });
            this.subscriptions.push(sb);
        }
    }

    clear() {
        const EmptyModel = {
            id: '',
            reportId: '00000000-0000-0000-0000-000000000000',
            period: '00000000-0000-0000-0000-000000000000',
            year: moment().year(),
            administrativeProceduresField: '00000000-0000-0000-0000-000000000000',
            totalReceive: 0,
            onlineInPeriod: 0,
            offlineInPeriod: 0,
            fromPreviousPeriod: 0,
            totalProcessed: 0,
            onTimeProcessed: 0,
            outOfDateProcessed: 0,
            beforeDeadlineProcessed: 0,
            totalProcessing: 0,
            onTimeProcessing: 0,
            outOfDateProcessing: 0,
        } as ReportAdministrativeProceduresModel;
        this.editData = EmptyModel;
        return EmptyModel;
    }

    loadForm() {
        this.formGroup = this.fb.group(
            {
                FieldId: [this.editData.administrativeProceduresField],
                Period: [this.editData.period],
                ReportYear: [this.editData.year, Validators.required],
                // Tiếp nhận
                OnlineInPeriod: [this.editData.onlineInPeriod, Validators.required],
                OfflineInPeriod: [this.editData.offlineInPeriod, Validators.required],
                FromPreviousPeriod: [this.editData.fromPreviousPeriod, Validators.required],
                // Đã giải quyết
                OnTimeProcessed: [this.editData.onTimeProcessed, Validators.required],
                OutOfDateProcessed: [this.editData.outOfDateProcessed, Validators.required],
                BeforeDeadlineProcessed: [this.editData.beforeDeadlineProcessed, Validators.required],
                // Đang giải quyết
                OnTimeProcessing: [this.editData.onTimeProcessing, Validators.required],
                OutOfDateProcessing: [this.editData.outOfDateProcessing, Validators.required],
            }
        )
        this.show = true;
    }

    private prepare() {
        const formValue = this.formGroup.value;
        this.editData.administrativeProceduresField = formValue.FieldId;
        this.editData.period = formValue.Period;
        this.editData.year = formValue.ReportYear;
        this.editData.onlineInPeriod = formValue.OnlineInPeriod;
        this.editData.offlineInPeriod = formValue.OfflineInPeriod;
        this.editData.fromPreviousPeriod = formValue.FromPreviousPeriod;
        this.editData.onTimeProcessed = formValue.OnTimeProcessed;
        this.editData.outOfDateProcessed = formValue.OutOfDateProcessed;
        this.editData.beforeDeadlineProcessed = formValue.BeforeDeadlineProcessed;
        this.editData.onTimeProcessing = formValue.OnTimeProcessing;
        this.editData.outOfDateProcessing = formValue.OutOfDateProcessing;
    }

    save() {
        this.prepare();
        if (this.id) {
            this.edit(this.editData);
        } else {
            this.create(this.editData);
        }
    }

    edit(item: any) {
        const sbUpdate = this.service.update(item).pipe(
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
        const sbCreate = this.service.create(item).pipe(
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

    getTotal(type: string) {
        const formData = this.formGroup.value;
        let result;
    
        switch (type) {
            case "Received":
                result = Number(formData.OnlineInPeriod) + Number(formData.OfflineInPeriod) + Number(formData.FromPreviousPeriod);
                break;
            case "Processed":
                result = Number(formData.OnTimeProcessed) + Number(formData.OutOfDateProcessed) + Number(formData.BeforeDeadlineProcessed);
                break;
            case "Processing":
                result = Number(formData.OnTimeProcessing) + Number(formData.OutOfDateProcessing);
                break;
            default:
                result = 0;
        }
    
        return result;
    }
    
}