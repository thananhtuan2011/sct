import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, finalize, first, tap } from 'rxjs/operators';
import Swal from 'sweetalert2';

import { NewRuralCriteriaModel } from '../../../_models/new-rural-criteria-page.model';
import { NewRuralCriteriaPageService } from '../../../_services/new-rural-criteria-page.service';
import { Options } from 'select2';
import { CommonService } from 'src/app/_metronic/shared/services/common.service';

@Component({
    selector: 'app-edit-modal',
    templateUrl: './edit-modal.component.html',
    styleUrls: ['./edit-modal.component.scss'],

})
export class EditNewRuralCriteriaModalComponent implements OnInit, OnDestroy {
    @Input() id: any;
    @Input() type: any;
    private subscriptions: Subscription[] = [];
    isLoading$: any;
    options: Options;
    editData: NewRuralCriteriaModel;
    formGroup: FormGroup;

    titleData: any = [
        {
            id: "0",
            text: "-- Chọn --"
        },
        {
            id: "1",
            text: "Nông thôn mới"
        },
        {
            id: "2",
            text: "Nông thôn mới nâng cao"
        },
    ];
    districtData: any = [];
    communeData: any = [];
    communeDataFilter: any = [];

    show: boolean = false;
    apiComplete: number = 0;

    constructor(
        private service: NewRuralCriteriaPageService,
        private commonService: CommonService,
        private fb: FormBuilder,
        public modal: NgbActiveModal,
        private cd: ChangeDetectorRef,
    ) { }

    ngOnInit(): void {
        this.isLoading$ = this.service.isLoading$;
        this.options = {
            theme: 'bootstrap5',
            templateSelection: this.templateSelection,
        };

        this.loadDistrict();
        this.loadCommune();
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
            this.districtData = data.sort((a, b) => a.text.localeCompare(b.text));
            this.loadData();
        })
    }

    loadCommune() {
        this.commonService.getCommune().subscribe((res: any) => {
            const data = [
                { id: "00000000-0000-0000-0000-000000000000", text: "-- Chọn --" },
                ...res.items.map((item: any) => ({
                    id: item.communeId,
                    text: item.communeName,
                    districtId: item.districtId
                }))
            ]
            this.communeData = data.sort((a, b) => a.text.localeCompare(b.text));
            this.communeDataFilter = data.sort((a, b) => a.text.localeCompare(b.text));
            this.loadData();
        })
    }

    loadData() {
        this.apiComplete++
        if (this.apiComplete != 2) {
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
                this.editData = res.items[0] as NewRuralCriteriaModel;
                this.loadForm();
            });
            this.subscriptions.push(sb);
        }
    }

    clear() {
        const EmptyModel = {
            newRuralCriteriaId: '00000000-0000-0000-0000-000000000000',
            districtId: '00000000-0000-0000-0000-000000000000',
            communeId: '00000000-0000-0000-0000-000000000000',
            titleIdStr: '0',
            target4: false,
            target7: false,
            target1708: false,
            note: '',
        } as NewRuralCriteriaModel;
        this.editData = EmptyModel;
        return EmptyModel;
    }

    loadForm() {
        this.formGroup = this.fb.group({
            DistrictId: [this.editData.districtId],
            CommuneId: [this.editData.communeId],
            Title: [this.editData.titleIdStr],
            Target4: [this.editData.target4],
            Target7: [this.editData.target7],
            Target1708: [this.editData.target1708],
            Note: [this.editData.note]
        })

        this.show = true;

        const districtChange = this.formGroup.controls.DistrictId.valueChanges.subscribe(x => {
            if (x != '00000000-0000-0000-0000-000000000000' && !this.type) {
                this.communeDataFilter = this.communeData.filter((y: any) => y.districtId == x || y.id == '00000000-0000-0000-0000-000000000000');
            } else {
                this.communeDataFilter = this.communeData;
            }
            if (!this.type) {
                this.formGroup.controls.CommuneId.setValue('00000000-0000-0000-0000-000000000000', { emitEvent: false });
            }
        });
        this.subscriptions.push(districtChange)

        if (this.type == "view") {
            this.formGroup.disable();
        }
    }

    private prepare() {
        const formData = this.formGroup.value;
        this.editData.districtId = formData.DistrictId;
        this.editData.communeId = formData.CommuneId;
        this.editData.titleIdStr = formData.Title;
        this.editData.target4 = formData.Target4;
        this.editData.target7 = formData.Target7;
        this.editData.target1708 = formData.Target1708;
        this.editData.note = formData.Note;
    }

    save() {
        this.prepare();
        if (this.id) {
            this.edit();
        } else {
            this.create();
        }
    }

    edit() {
        const sbUpdate = this.service.update(this.editData).pipe(
            tap(() => {
                this.modal.close();
            }),
            catchError((errorMessage) => {
                this.modal.dismiss(errorMessage);
                return of(this.editData);
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

    create() {
        const sbCreate = this.service.create(this.editData).pipe(
            tap(() => {
                this.modal.close();
            }),
            catchError((errorMessage) => {
                this.modal.dismiss(errorMessage);
                return of(this.editData);
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

    check_formGroup() {
        this.formGroup.markAllAsTouched();
        if (!this.formGroup.invalid) {
            this.save()
        }
    }
}
