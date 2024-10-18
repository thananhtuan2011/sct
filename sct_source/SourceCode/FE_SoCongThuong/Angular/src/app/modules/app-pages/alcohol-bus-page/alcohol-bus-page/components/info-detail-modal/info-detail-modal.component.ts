import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { AlcoholBusinessModel } from '../../../_models/alcohol-bus.model';
import { AlcoholBusinessPageService } from '../../../_services/alcohol-bus-page.service';

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
  selector: 'app-info-detail-modal.component',
  templateUrl: './info-detail-modal.component.html',
  styleUrls: ['./info-detail-modal.component.scss'],
})
export class InfoAlcoholBusinessDetailModalComponent implements OnInit {
  @Input() props: any;
  isLoading$: any;
  alcoholBusinessData: AlcoholBusinessModel;

  formGroup: FormGroup;
  id = 0;
  data: any = {};

  constructor(
    private alcoholBusinessService: AlcoholBusinessPageService,
    private fb: FormBuilder,
    public modal: NgbActiveModal
  ) {}

  ngOnInit(): void {
    this.isLoading$ = this.alcoholBusinessService.isLoading$;
    this.loadDetail();
  }

  delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }
  loadDetail() {
    this.data = this.props;
    this.loadForm();
  }
  loadForm() {
    this.formGroup = this.fb.group({
      TenDoanhNghiep: [this.data.tenDoanhNghiep],
      NguoiDaiDien: [this.data.nguoiDaiDien],
      SoDienThoai: [this.data.soDienThoai],
      Huyen: [this.data.tenHuyen],
      Xa: [this.data.tenXa],
      DiaChi: [this.data.diaChi],
      GiayPhepKinhDoanh: [this.data.giayPhepKinhDoanh],
      NgayHetHan: this.convert_date_string(this.data.ngayHetHan),
      DonViCungCap: [this.data.donViCungCap],
      DiaChiDonViCungCap: this.data.diaChiDonViCungCap,
      SoDienThoaiDonViCungCap: this.data.soDienThoaiDonViCungCap,
      NgayCapGiayPhepBanLe: this.convert_date_string(
        this.data.ngayCapGiayPhepBanLe
      ),
      GhiChu: this.data.ghiChu
    });
  }

  convert_date_string(string_date: string) {
    if (string_date == null) return '';
    var date = string_date.split('T')[0];
    var list = date.split('-'); //["year", "month", "day"]
    var result = list[2] + '/' + list[1] + '/' + list[0];
    return result;
  }
}
