import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { CigaretteBusinessModel } from '../../../_models/cigarette-bus.model';
import { CigaretteBusinessPageService } from '../../../_services/cigarette-bus-page.service';
import { InfoCigaretteBusinessDetailModalComponent } from '../info-detail-modal/info-detail-modal.component';

const EMPTY_CUSTOM: CigaretteBusinessModel = {
  id: '',
  CigaretteBusinessId: '',
  cigaretteBusinessName: '00000000-0000-0000-0000-000000000000',
  cigaretteBusinessDetail: []
};

@Component({
  selector: 'app-info-page-modal.component',
  templateUrl: './info-page-modal.component.html',
  styleUrls: ['./info-page-modal.component.scss'],

})
export class InfoCigaretteBusinessModalComponent implements OnInit {
  @Input() id: any;
  isLoading$:any;
  cigaretteBusinessData: CigaretteBusinessModel;
  formGroup: FormGroup;
  dataSource: any[] = [];
	lstStore: any[] = [];

  data : any = {};
  private subscriptions: Subscription[] = [];

  constructor(
    private cigaretteBusinessService: CigaretteBusinessPageService,
    private fb: FormBuilder, public modal: NgbActiveModal,
    private modalService: NgbModal
    ) {}

  ngOnInit(): void {
    this.isLoading$ = this.cigaretteBusinessService.isLoading$;
    this.loadDetail();
  }

  loadDetail() {
    if (!this.id) {
      this.loadForm();
    } else {
      const sb = this.cigaretteBusinessService.getItemById(this.id).pipe(
        first(),
        catchError((errorMessage) => {
          this.modal.dismiss(errorMessage);
          return of(EMPTY_CUSTOM);
        })
      ).subscribe((res: any) => {
        this.cigaretteBusinessData = res.items[0];
        this.data = res.items[0];
        this.dataSource=res.items[0].cigaretteBusinessDetail;    
        this.loadForm();
      });
      this.subscriptions.push(sb);
    }
  }
  loadForm() {
    this.formGroup = this.fb.group({
      cigaretteBusinessName: [this.data.cigaretteBusinessName],
      TenDoanhNghiep : [this.data.businessNameVi],
      NguoiDaiDien : [this.data.representative],
      SoDienThoai: [this.data.phoneNumber],
      DiaChiTruSo: [this.data.address],
      NgayCapPhep: this.convert_date_string(this.data.ngayCap),
      GiayDangKyKinhDoanh: this.data.giayDangKyKinhDoanh
    });
  }
  
  view(item: any) {
    const modalRef = this.modalService.open(InfoCigaretteBusinessDetailModalComponent, { size: 'xl' });
    modalRef.componentInstance.props = item;
    modalRef.result.then(() =>
      this.cigaretteBusinessService.fetch(),
      () => { }
    );
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
