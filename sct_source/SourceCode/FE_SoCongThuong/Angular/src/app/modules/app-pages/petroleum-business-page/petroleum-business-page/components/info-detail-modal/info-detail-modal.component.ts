import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { PetroleumBusinessModel } from '../../../_models/petroleum-business.model';
import {PetroleumBusinessPageService } from '../../../_services/petroleum-business-page.service';

const EMPTY_CUSTOM: PetroleumBusinessModel = {
  id: '',
  petroleumBusinessId: '',
  petroleumBusinessName: '00000000-0000-0000-0000-000000000000',
  petroleumBusinessDetail: [],
  giayDangKyKinhDoanh: '',
  ngayCap: null
};

@Component({
  selector: 'app-info-detail-modal.component',
  templateUrl: './info-detail-modal.component.html',
  styleUrls: ['./info-detail-modal.component.scss'],

})
export class InfoPetroleumBusinessDetailModalComponent implements OnInit {
  @Input() props: any;
  isLoading$:any;
  alcoholBusinessData: PetroleumBusinessModel;

  formGroup: FormGroup;
  id = 0
  data : any = {};


  constructor(
    private petroleumBusinessService: PetroleumBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    ) {}

  ngOnInit(): void{
    this.isLoading$ = this.petroleumBusinessService.isLoading$;
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
      TenCuaHang : [this.data.tenCuaHang],
      NguoiDaiDien : [this.data.nguoiDaiDien],
      SoDienThoai: [this.data.soDienThoai],
      Huyen: [this.data.tenHuyen],
      Xa: [this.data.tenXa],
      DiaChi: [this.data.diaChi],
      GiayPhepKinhDoanh: [this.data.giayPhepKinhDoanh],
      NgayHetHan: this.data.ngayHetHan != null ? this.convert_date_string(this.data.ngayHetHan) : null,
      DonViCungCap: [this.data.donViCungCap],
      NgayCapPhep : this.data.ngayCapPhep != null ? this.convert_date_string(this.data.ngayCapPhep) : null,
      ThoiHan5Nam : this.data.thoiHan5Nam != null ? this.convert_date_string(this.data.thoiHan5Nam) : null,
      NguoiQuanLy : [this.data.nguoiQuanLy],
      HinhThuc : [this.data.tenHinhThuc],
      SoCotBomE5 :[this.data.soCotBomE5],
      SoCotBomA95 :[this.data.soCotBomA95],
      SoCotBomOil :[this.data.soCotBomOil],
      SoBeChua :[this.data.soBeChua],
      TongDungTich :[this.data.tongDungTich],
      ThoiGianBanHang : [this.data.thoiGianBanHang],
      DienTichXayDung :[this.data.dienTichXayDung],
      TuyenPhucVu :[this.data.tuyenPhucVu],
      ThoiHan1Name: this.data.thoiHan1Nam != null ? this.convert_date_string(this.data.thoiHan1Nam): null,
      NguoiLienHeDonViCungCap: this.data.nguoiLienHeDonViCungCap,
      SoDienThoaiDonViCungCap: this.data.soDienThoaiDonViCungCap,
      DiaChiDonViCungCap: this.data.diaChiDonViCungCap,
      GhiChu: this.data.ghiChu,
      LoaiGiayXacNhan: this.data.tenLoaiGiayXacNhan,
      HinhThucHopDong: this.data.tenHinhThucHopDong,
      NgayCapPhepXayDung: this.data.ngayCapPhepXayDung != null ? this.convert_date_string(this.data.ngayCapPhepXayDung) : null
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
