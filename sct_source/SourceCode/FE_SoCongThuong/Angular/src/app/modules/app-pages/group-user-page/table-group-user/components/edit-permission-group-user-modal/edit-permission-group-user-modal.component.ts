import { Component, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { catchError, of, tap } from 'rxjs';
import Swal from 'sweetalert2';
import { GroupUserService } from '../../../_services/group-user.service';

@Component({
  selector: 'app-edit-permission-group-user-modal',
  templateUrl: './edit-permission-group-user-modal.component.html',
  styleUrls: ['./edit-permission-group-user-modal.component.scss']
})
export class EditPermissionGroupUserModalComponent implements OnInit {
  isLoading$:any;
  SelectedList: any[] = [];
  dataTreeQuyen: any[] = [
	];
  @Input() id: number;
  constructor(public modal: NgbActiveModal,
    private UserService: GroupUserService,
    private changeDetectorRefs: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.GetTreePhanQuyen()
  }

  treequyenChanged(e: any) {
		this.SelectedList = e.value;
	}

  GetTreePhanQuyen() {
		this.UserService.GetTreePhanQuyen(this.id).subscribe((res:any) => {
			var alldata = res.data;
			this.dataTreeQuyen.push(alldata);
			for (var i = 0; i < res.data_role.length; i++) {
				this.SelectedList.push({ idRole: res.data_role[i].code });
			}
			this.changeDetectorRefs.detectChanges();
		});
	}

  saveRoles(){
    const list: string[] = [];
    for (var i = 0; i < this.SelectedList.length; i++) {
      list.push(this.SelectedList[i].idRole);
    }
    let itemGroup: any = {};
    itemGroup.IdGroup = this.id;//_PhanQuyenUserGroup.IdGroup;
    itemGroup.Code = list ;
    this.UserService.updateQuyenNhomNguoiDung(itemGroup).pipe(
      tap(() => {
        this.modal.close();
      }),
      catchError((errorMessage) => {
        Swal.fire({
          icon: 'error',
          title: 'Thông báo lỗi',
          confirmButtonText: 'Xác nhận',
           text: errorMessage.error.error.msg,
        });
        this.modal.dismiss(errorMessage);
        return of({});
      }),
    ).subscribe(res => {
      if(res.status == 1){
        Swal.fire({
          icon: res.status == 1 ? 'success' : 'error',
          title: 'Cập nhật quyền thành công',
          confirmButtonText: 'Xác nhận',
          text: 'Cập nhật ' + (res.status == 1 ? 'thành công' : 'thất bại'),
        });
      }
    })
  }
}
