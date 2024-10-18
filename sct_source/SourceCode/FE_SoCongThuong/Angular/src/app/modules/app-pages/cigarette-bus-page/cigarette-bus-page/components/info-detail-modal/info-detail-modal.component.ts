import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { CigaretteBusinessModel } from '../../../_models/cigarette-bus.model';
import { CigaretteBusinessPageService } from '../../../_services/cigarette-bus-page.service';

const EMPTY_CUSTOM: CigaretteBusinessModel = {
  id: '',
  CigaretteBusinessId: '',
  cigaretteBusinessName: '00000000-0000-0000-0000-000000000000',
  cigaretteBusinessDetail: []
};

@Component({
  selector: 'app-info-detail-modal.component',
  templateUrl: './info-detail-modal.component.html',
  styleUrls: ['./info-detail-modal.component.scss'],

})
export class InfoCigaretteBusinessDetailModalComponent implements OnInit {
  @Input() props: any;
  isLoading$:any;
  cigaretteBusinessData: CigaretteBusinessModel;
  
  formGroup: FormGroup;
  id = 0
  data : any = {};


  constructor(
    private cigaretteBusinessService: CigaretteBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) {}

  ngOnInit(): void{
    this.isLoading$ = this.cigaretteBusinessService.isLoading$;
    this.loadDetail();
  }

  delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
  loadDetail() {
    this.data = this.props
    this.loadForm()
  }
  loadForm() {
    this.formGroup = this.fb.group({
      TenDoanhNghiep : [this.data.tenDoanhNghiep],
      NguoiDaiDien : [this.data.nguoiDaiDien],
      SoDienThoai: [this.data.soDienThoai],
      Huyen: [this.data.tenHuyen],
      Xa: [this.data.tenXa],
      DiaChi: [this.data.diaChi],
      GiayPhepKinhDoanh: [this.data.giayPhepKinhDoanh],
      NgayHetHan: this.convert_date_string(this.data.ngayHetHan),
      DonViCungCap: [this.data.donViCungCap],
      PhoneDonViCungCap: [this.data.phoneDonViCungCap],
      DiaChiDonViCungCap: [this.data.diaChiDonViCungCap],
      NgayCap: this.convert_date_string(this.data.ngayCap),
      GhiChu: this.data.ghiChu
    });
  }
  
  convert_date_string(string_date: string) {
    if(string_date == null)
    return ''
    var date = string_date.split("T")[0];
    var list = date.split("-"); //["year", "month", "day"]
    var result = list[2] + "/" + list[1] + "/" + list[0]
    return result
  }

}
