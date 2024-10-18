import { style } from '@angular/animations';
import { Component, Input, OnInit } from '@angular/core';
import { CategoryImportService } from '../../../_services/category-import.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';
import { catchError, finalize, of, tap } from 'rxjs';
import { indexOf } from 'lodash';

@Component({
  selector: 'app-view-category-import-modal',
  templateUrl: './view-category-import-modal.component.html',
  styleUrls: ['./view-category-import-modal.component.scss']
})
export class ViewCategoryImportModalComponent implements OnInit {
  @Input() data: FormData;
  DuLieu: any = [];
  disableButton: boolean = false;
  columns_remove: any = [];
  columns_error: any = [];
  constructor(private categoryImportService: CategoryImportService,
    public modal: NgbActiveModal,) { }

  ngOnInit(): void {
    this.categoryImportService.importFileExcel(this.data).subscribe(res => {
      if (res) {
        this.DuLieu = res.data
      }
    })
  }

  checkRow(column: any) {
    if (this.columns_error.indexOf(column) != -1) {
      return true;
    }
    return false;
  }

  checkVal(vaule: any, columns: any) {
    var firstChar = vaule.substring(0, 1);

    if (firstChar === '2') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Không được để trống</strong>`;
    }

    if (firstChar === '3') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Dữ liệu đã tồn tại</strong>`;
    }

    if (firstChar === '4') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Dữ liệu không tồn tại trong bảng liên kết</strong>`;
    }
    if (firstChar === '5') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Bảng dữ liệu tham chiếu không đúng định dạng</strong>`;
    }

    if (firstChar === '6') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Dữ liệu phải là kiểu text</strong>`;
    }

    if (firstChar === '7') {
      this.disableButton = true;
      this.columns_error.push(columns);
      return `<strong style="color:red"> Dữ liệu phải là kiểu date</strong>`;
    }

    var fir = vaule.substring(1)
    return fir.substring(0, fir.indexOf("%%"));
  }

  // checkValBefDe(vaule: any) {
  //   var firstChar = vaule.substring(0, 1);
  //   if (firstChar === '2') {
  //     this.disableButton = true
  //   } else
  //     if (firstChar === '3') {
  //       this.disableButton = true
  //     } else
  //       if (firstChar === '4') {
  //         this.disableButton = true
  //       } else
  //         if (firstChar === '5') {
  //           this.disableButton = true
  //         } else
  //           if (firstChar === '6') {
  //             this.disableButton = true
  //           } else
  //             if (firstChar === '7') {
  //               this.disableButton = true
  //             } else this.disableButton = false
  // }
  checkValBefDe(vaule: any) {
    var firstChar = vaule.substring(0, 1);

    switch (firstChar) {
      case '2':
      case '3':
      case '4':
      case '5':
      case '6':
      case '7':
        this.disableButton = true;
        break;
      default:
        this.disableButton = false;
        break;
    }
  }

  deleteRow(index: any) {
    debugger
    let delRow = this.DuLieu.rows[index][0].split("%%")[1];
    // var lastChar = (this.DuLieu.rows[index][0].charAt(this.DuLieu.rows[index][0].indexOf("%%"))).replace("%%", "");
    this.columns_remove.push(Number(delRow))
    this.DuLieu.rows.splice(index, 1)
    this.DuLieu.rows.forEach((res: any) => {
      for (let index = 0; index < this.DuLieu.columns.length; index++) {
        const element = res[index];
        this.checkValBefDe(element)
      }
    });
    if (this.DuLieu.rows.length == 0) {
      this.disableButton = true
    }
  }

  import() {
    this.data.append("remove", JSON.stringify(this.columns_remove));
    this.categoryImportService.importFileExcelSave(this.data).pipe(
      tap(() => {
        // this.modal.close();
      }),
      catchError((errorMessage) => {
        this.modal.dismiss(errorMessage);
        return of(undefined);
      }),
    ).subscribe((res: any) => {
      this.modal.dismiss(res);
    });
  }

}
