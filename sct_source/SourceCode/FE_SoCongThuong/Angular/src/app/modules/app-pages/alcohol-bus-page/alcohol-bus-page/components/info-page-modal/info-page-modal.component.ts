import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { AlcoholBusinessModel } from '../../../_models/alcohol-bus.model';
import { AlcoholBusinessPageService } from '../../../_services/alcohol-bus-page.service';
import { InfoAlcoholBusinessDetailModalComponent } from '../info-detail-modal/info-detail-modal.component';

const EMPTY_CUSTOM: AlcoholBusinessModel = {
  id: '',
  alcoholBusinessId: '',
  alcoholBusinessName: '00000000-0000-0000-0000-000000000000',
  alcoholBusinessDetail: [],
  giayDangKyKinhDoanh: '',
  ngayCapPhep: null,
  giayPhepBanBuon: '',
  ngayCapGiayPhepBanBuon: null,
  ngayHetHanGiayPhepBanBuon: null,
};

@Component({
  selector: 'app-info-page-modal.component',
  templateUrl: './info-page-modal.component.html',
  styleUrls: ['./info-page-modal.component.scss'],
})
export class InfoAlcoholBusinessModalComponent implements OnInit {
  @Input() id: any;
  isLoading$: any;
  alcoholBusinessData: AlcoholBusinessModel;
  formGroup: FormGroup;
  dataSource: any[] = [];
  lstStore: any[] = [];

  data: any = {};
  private subscriptions: Subscription[] = [];

  constructor(
    private alcoholBusinessService: AlcoholBusinessPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal,
    private modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.alcoholBusinessService.isLoading$;
    this.loadDetail();
  }

  loadDetail() {
    if (!this.id) {
      this.loadForm();
    } else {
      const sb = this.alcoholBusinessService
        .getItemById(this.id)
        .pipe(
          first(),
          catchError((errorMessage) => {
            this.modal.dismiss(errorMessage);
            return of(EMPTY_CUSTOM);
          })
        )
        .subscribe((res: any) => {
          this.alcoholBusinessData = res.items[0];
          this.data = res.items[0];
          this.dataSource = res.items[0].alcoholBusinessDetail;
          this.loadForm();
        });
      this.subscriptions.push(sb);
    }
  }
  loadForm() {
    this.formGroup = this.fb.group({
      AlcoholBusinessName: [this.data.alcoholBusinessName],
      TenDoanhNghiep: [this.data.businessNameVi],
      NguoiDaiDien: [this.data.representative],
      SoDienThoai: [this.data.phoneNumber],
      DiaChiTruSo: [this.data.address],
      GiayDangKyKinhDoanh: this.data.giayDangKyKinhDoanh,
      NgayCapPhep: this.convert_date_string(this.data.ngayCapPhep),
      GiayPhepBanBuon: this.data.giayPhepBanBuon,
      NgayCapGiayPhepBanBuon: this.convert_date_string(
        this.data.ngayCapGiayPhepBanBuon
      ),
      NgayHetHanGiayPhepBanBuon: this.convert_date_string(
        this.data.ngayHetHanGiayPhepBanBuon
      ),
    });
  }

  view(item: any) {
    const modalRef = this.modalService.open(
      InfoAlcoholBusinessDetailModalComponent,
      { size: 'xl' }
    );
    modalRef.componentInstance.props = item;
    modalRef.result.then(
      () => this.alcoholBusinessService.fetch(),
      () => {}
    );
  }

  convert_date_string(string_date: string | null) {
    if (string_date === null) {
      return null;
    }
    var date = string_date.split('T')[0];
    var list = date.split('-'); //["year", "month", "day"]
    var result = list[2] + '/' + list[1] + '/' + list[0];
    return result;
  }
}
