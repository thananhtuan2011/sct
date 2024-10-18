import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { PetroleumBusinessModel } from '../../../_models/petroleum-business.model';
import { PetroleumBusinessPageService } from '../../../_services/petroleum-business-page.service';
import { InfoPetroleumBusinessDetailModalComponent } from '../info-detail-modal/info-detail-modal.component';

const EMPTY_CUSTOM: PetroleumBusinessModel = {
  id: '',
  petroleumBusinessId: '',
  petroleumBusinessName: '00000000-0000-0000-0000-000000000000',
  petroleumBusinessDetail: [],
  giayDangKyKinhDoanh: '',
  ngayCap: null
};

@Component({
  selector: 'app-info-page-modal.component',
  templateUrl: './info-page-modal.component.html',
  styleUrls: ['./info-page-modal.component.scss'],

})
export class InfoPetroleumBusinessModalComponent implements OnInit {
  @Input() id: any;
  isLoading$:any;
  petroleumBusinessData: PetroleumBusinessModel;
  formGroup: FormGroup;
  dataSource: any[] = [];
	lstStore: any[] = [];

  data : any = {};
  private subscriptions: Subscription[] = [];

  constructor(
    private petroleumBusinessService: PetroleumBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal
    ) {}

  ngOnInit(): void {
    this.isLoading$ = this.petroleumBusinessService.isLoading$;
    this.loadDetail();
  }

  loadDetail() {
    if (!this.id) {
      this.loadForm();
    } else {
      const sb = this.petroleumBusinessService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.petroleumBusinessData = res.items[0];
        this.data = res.items[0];
        this.dataSource=res.items[0].petroleumBusinessDetail;
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }
  loadForm() {
    this.formGroup = this.fb.group({
      PetroleumBusinessName: [this.data.petroleumBusinessName],
      TenDoanhNghiep : [this.data.businessNameVi],
      NguoiDaiDien : [this.data.representative],
      SoDienThoai: [this.data.phoneNumber],
      DiaChiTruSo: [this.data.address],
      GiayDangKyKinhDoanh: this.data.giayDangKyKinhDoanh,
      NgayCap: this.data.ngayCap != null ? this.convert_date_string(this.data.ngayCap) : null
    });
  }
  
  convert_date_string(string_date: string) {
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

  view(item: any) {
    const modalRef = this.modalService.open(InfoPetroleumBusinessDetailModalComponent, { size: 'xl' });
    modalRef.componentInstance.props = item;
    modalRef.result.then(() =>
      this.petroleumBusinessService.fetch(),
      () => { }
    );
  }
}
