import { environment } from 'src/environments/environment';
import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatMenuTrigger } from '@angular/material/menu';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Options } from 'select2';
import { SelectOptionData } from 'src/app/_metronic/shared/components/select-custom/select-custom.interface';
import { CategoryImportService } from '../_services/category-import.service';
import { ViewCategoryImportModalComponent } from './components/view-category-import-modal/view-category-import-modal.component';
import Swal from 'sweetalert2';
import { catchError, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-table-category-import-page',
  templateUrl: './table-category-import-page.component.html',
  styleUrls: ['./table-category-import-page.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class TableCategoryImportPageComponent implements OnInit {
  DataDanhMuc: Array<SelectOptionData>;
  public options: Options;
  isShow: boolean = true;
  isLoading: boolean = true;
  isLoadingDanhMuc: boolean = false;
  private fileUpload: ElementRef;
  @ViewChild('fileUpload', { static: false }) set content(content: ElementRef) {
    if(content) {
      this.fileUpload = content;
    }
  }
  @ViewChild(MatMenuTrigger, { static: true }) matMenuTriggerCol: MatMenuTrigger;
  menuTopLeftPosition = { x: '0', y: '0' }

  file_documents: any[] = [];
  lstCols: any
  data_Map: any = []
  filterGroup: FormGroup;
  listDanhMuc: any = []
  idDanhMuc: any
  valueState: any

  constructor(
    private categoryImportService: CategoryImportService,
    private fb: FormBuilder,
    private modalService: NgbModal,
    private router: Router,
    private http: HttpClient) { }

  ngOnInit(): void {
    this.filterGroup = this.fb.group({
      danhmuc: ["0"],
    });
    const data = [
      {
        id: "0",
        text: '-- Chọn --'
      }
    ]
    this.categoryImportService.loadListDanhMuc().subscribe(res => {
      if (res.data.length > 0) {
        res.data.forEach((element: any) => {
          var dt = {
            id: element.id,
            text: element.name
          }
          data.push(dt)
        });
        this.DataDanhMuc = data
        this.categoryImportService.loadItemMucByUrl(this.getParams()).subscribe(res => {
          if (res.data.length > 0) {
            this.change_value(res.data[0].id, 'danhmuc')
          } else {
            this.filterGroup.controls["danhmuc"].setValue("0");
          }
          this.isLoading = false;
          this.isShow = false;
        })
      }
    })

    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection
    }
  }

  getParams() {
    const url = window.location.href;
    let paramValue = undefined;
    if (url.includes('?')) {
      const httpParams = new HttpParams({ fromString: url.split('?')[1] });
      paramValue = httpParams.get('url');
    }
    return paramValue;
  }

  // function for selection template
  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  change_value(event: any, ControlName: string) {
    this.filterGroup.controls[ControlName].setValue(event);
    this.idDanhMuc = event
    this.isLoadingDanhMuc = true
    this.categoryImportService.loadColsDanhMucById(event).subscribe(res => {
      if (res) {
        this.data_Map = []
        this.listDanhMuc = res.data
        this.isLoadingDanhMuc = false
      }
    })
  }

  loadColsFile() {
    var formData: any = new FormData();
    //File documents
    if (this.file_documents.length > 0) {
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("", document);
        }
      }
    }
    this.isLoading = true
    this.categoryImportService.loadFileExcel(formData).subscribe(res => {
      if (res) {
        this.lstCols = res;
        this.isLoading = false
      }
    })
  }
  triggerClick() {
    let ele = this.fileUpload.nativeElement;
    ele.click();
  }

  fileBrowseHandler(files: any) {
    this.prepareFilesList(files.target.files);
  }

  prepareFilesList(files: Array<any>) {
    for (const item of files) {
      this.file_documents.push(item);
      this.loadColsFile()
    }
  }
  deleteFile(index: number) {
    this.file_documents.splice(index, 1);
    this.lstCols = []
    this.fileUpload.nativeElement.value = "";
    this.data_Map = []
  }

  clickCol(value: any) {
    if (this.data_Map.length > 0) {
      if (this.data_Map[this.data_Map.length - 1].idRowMap === "") return
    }

    var obj = this.data_Map.find((x: any) => x.idCol === value.idCol)
    if (obj) return
    var data = {
      idCol: value.idCol,
      nameCol: value.nameCol,
      idRowMap: "",
      nameMap: "",
      type: this.idDanhMuc,
      required: value.nameCol.includes('*'),
      ref: '',
      refCol: '',
      refId: '',
      isNull: true
    }
    this.data_Map.push(data)
  }

  clickColMap(value: any) {
    if (this.valueState) {
      var index = this.data_Map.findIndex((x: any) => x.idCol === this.valueState)
      this.data_Map[index].idRowMap = value.id
      this.data_Map[index].nameMap = value.name
      this.data_Map[index].type = this.idDanhMuc
      this.data_Map[index].required = value.name.includes('*')
      this.data_Map[index].isNull = value.isNull
      this.data_Map[index].refCol = value.refCol
      this.data_Map[index].refId = value.refId
      this.data_Map[index].ref = value.refTable
    } else
      if (this.data_Map.length > 0) {
        this.data_Map[this.data_Map.length - 1].idRowMap = value.id
        this.data_Map[this.data_Map.length - 1].nameMap = value.name
        this.data_Map[this.data_Map.length - 1].type = this.idDanhMuc
        this.data_Map[this.data_Map.length - 1].required = value.name.includes('*')
        this.data_Map[this.data_Map.length - 1].isNull = value.isNull
        this.data_Map[this.data_Map.length - 1].refCol = value.refCol
        this.data_Map[this.data_Map.length - 1].refId = value.refId
        this.data_Map[this.data_Map.length - 1].ref = value.refTable
      }
  }

  ThemRowSelect() {

  }
  thongbao() {
    Swal.fire({
      icon: 'error',
      title: 'Thất bại',
      confirmButtonText: 'Xác nhận',
      text: 'Vui lòng liên kết các trường dữ liệu',
    });
  }
  view() {
    if (this.data_Map.length === 0) {
      this.thongbao()
      return
    }
    if (this.data_Map.length > 0) {
      if (this.data_Map[0].ref === '') {
        this.thongbao()
        return
      }

    }
    const modalRef = this.modalService.open(ViewCategoryImportModalComponent, { size: 'xl', centered: true });
    var formData: any = new FormData();
    //File documents
    formData.append("data", JSON.stringify(this.data_Map));
    if (this.file_documents.length > 0) {
      for (var document of this.file_documents) {
        if (document.name) {
          formData.append("Files", document);
        }
      }
    }
    modalRef.componentInstance.data = formData;
    modalRef.result.then(({ ...res }) =>
      res,
      (res) => {
        if (res) {
          if (res.status == 1) {
            Swal.fire({
              title: 'Import thành công',
              text: 'Bạn có muốn chuyển hướng về lại trang danh sách không ?',
              icon: 'success',
              showCancelButton: true,
              confirmButtonColor: '#3085d6',
              cancelButtonColor: '#d33',
              confirmButtonText: 'Xác nhận',
              cancelButtonText: 'Thoát'
            }).then((result) => {
              if (result.isConfirmed) {
                this.router.navigate(['/' + this.getParams()])
              }
            })
          } else {
            Swal.fire({
              icon: res.status == 1 ? 'success' : 'error',
              title: res.status == 1 ? 'Import thành công' : 'Import thất bại',
              confirmButtonText: 'Xác nhận',
              text: res.status == 1 ? 'Import thành công' : res.status == 0 ? res.error.msg : 'Import thất bại',
            });
          }
        }
      }
    );
  }

  onRightClick(event: MouseEvent, item: any) {
    event.preventDefault();
    this.menuTopLeftPosition.x = event.clientX + 'px';
    this.menuTopLeftPosition.y = event.clientY + 'px';
    this.matMenuTriggerCol.menuData = { item: item }
    this.matMenuTriggerCol.openMenu();
  }

  removeCol(item: any) {
    var index = this.data_Map.findIndex((x: any) => x.idCol === item.idCol)
    if (index > -1) {
      this.data_Map.splice(index, 1);
    }
  }

  removeColMap(item: any) {
    var index = this.data_Map.findIndex((x: any) => x.idRowMap === item.idRowMap)
    if (index > -1) {
      this.data_Map[index].idRowMap = ""
      this.data_Map[index].nameMap = ""
      this.data_Map[index].type = ""
      this.data_Map[index].ref = ""
    }
  }

  selectedCol(event: any) {
    this.valueState = event.selectedOptions.selected[0].value
  }

  change_valueRe(event: any, col: any) {
    var index = this.data_Map.findIndex((x: any) => x.idCol === col.idCol)
    this.data_Map[index].ref = event
  }

  exportFile() {

    let dm = this.DataDanhMuc.find(x => x.id === this.idDanhMuc)?.text
    const now = new Date();
    const timeString = now.toLocaleString('en-US', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).replace(/\D/g, '');

    var exten = ".xlsx"
    if (this.convertToUnsigned(dm) === 'cacchitieusanxuatkinhdoanhxuatkhauchuyeu' || this.convertToUnsigned(dm) === 'quanlydanhsachduan') {
      exten = ".zip"
    }
    const fileName = this.convertToUnsigned(dm) + exten

    Swal.fire({
      title: 'Đang xuất File...',
      // text: 'Vui lòng đợi một lúc trước khi file của bạn sẵn sàng!',
      didOpen: () => {
        Swal.showLoading()
      },
    })

    const query = {
      filter: {},
      grouping: {},
      paginator: {},
      sorting: { column: "id", direction: "desc" },
      searchTerm: this.idDanhMuc
    }

    this.http.post(`${environment.apiUrl}/import/Export`, query,
      {
        responseType: 'blob',
      }).pipe(
        catchError((errorMessage: any) => {
          console.error(errorMessage)
          Swal.fire({
            icon: 'error',
            title: 'File không tồn tại',
            confirmButtonText: 'Xác nhận',
          });
          return of();
        }),
      ).subscribe(
        (res) => {
          let type = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
          if (this.convertToUnsigned(dm) === 'cacchitieusanxuatkinhdoanhxuatkhauchuyeu' || this.convertToUnsigned(dm) === 'quanlydanhsachduan') {
            type = "application/octet-stream"
          }
          const file = new Blob([res], { type: type });
          const fileURL = URL.createObjectURL(file);
          const a = document.createElement('a');
          a.href = fileURL;
          a.download = fileName;
          document.body.append(a);
          a.click();
          a.remove();
          URL.revokeObjectURL(fileURL);
          Swal.fire({
            icon: 'success',
            title: 'Download File thành công',
            confirmButtonText: 'Xác nhận',
          });
        },
      );
  }

  convertToUnsigned(str: any) {
    const charTable = {
      'à': 'a', 'á': 'a', 'ả': 'a', 'ã': 'a', 'ạ': 'a',
      'ă': 'a', 'ằ': 'a', 'ắ': 'a', 'ẳ': 'a', 'ẵ': 'a', 'ặ': 'a',
      'â': 'a', 'ầ': 'a', 'ấ': 'a', 'ẩ': 'a', 'ẫ': 'a', 'ậ': 'a',
      'đ': 'd',
      'è': 'e', 'é': 'e', 'ẻ': 'e', 'ẽ': 'e', 'ẹ': 'e',
      'ê': 'e', 'ề': 'e', 'ế': 'e', 'ể': 'e', 'ễ': 'e', 'ệ': 'e',
      'ì': 'i', 'í': 'i', 'ỉ': 'i', 'ĩ': 'i', 'ị': 'i',
      'ò': 'o', 'ó': 'o', 'ỏ': 'o', 'õ': 'o', 'ọ': 'o',
      'ô': 'o', 'ồ': 'o', 'ố': 'o', 'ổ': 'o', 'ỗ': 'o', 'ộ': 'o',
      'ơ': 'o', 'ờ': 'o', 'ớ': 'o', 'ở': 'o', 'ỡ': 'o', 'ợ': 'o',
      'ù': 'u', 'ú': 'u', 'ủ': 'u', 'ũ': 'u', 'ụ': 'u',
      'ư': 'u', 'ừ': 'u', 'ứ': 'u', 'ử': 'u', 'ữ': 'u', 'ự': 'u',
      'ỳ': 'y', 'ý': 'y', 'ỷ': 'y', 'ỹ': 'y', 'ỵ': 'y',
    };
    let result = '';
    for (let i = 0; i < str.length; i++) {
      const ch: keyof typeof charTable = str[i];
      const uch = charTable[ch];
      result += (uch !== undefined) ? uch : ch;
    }

    result = result.replace(/[^\w\s]/gi, '');
    result = result.toLowerCase();

    return result.replace(/\s+/g, '');
  }
}
