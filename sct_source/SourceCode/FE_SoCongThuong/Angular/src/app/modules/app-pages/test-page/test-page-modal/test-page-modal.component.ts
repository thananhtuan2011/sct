import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { DemoModel } from '../test-page-models/test-page-model';
import { TestPageService } from '../test-page.service';

@Component({
  selector: 'app-test-page-modal',
  templateUrl: './test-page-modal.component.html'
})
export class TestPageModalComponent implements OnInit {
  @Input() public data: DemoModel;// data nhận từ hàm mở modal
  Form: FormGroup;// khởi tạo form
  hasFormErrors: boolean = false;
  _data: DemoModel = new DemoModel();
  constructor(
    private readonly activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private detectchanges: ChangeDetectorRef,
    private _TestPageService: TestPageService
  ) { }

  ngOnInit(): void {
    this._data = this.data;
    this.initForm();
    this.detectchanges.detectChanges();
  }
  initForm() {
    this.Form = this.fb.group({
      Name: [this._data.name, Validators.required],
    });
  }/// gắn dữ liệu lên form
  onSubmit() {
		this.hasFormErrors = false;
		const controls = this.Form.controls;
		if (this.Form.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);

			this.hasFormErrors = true;
			return;
		}// kiểm tra tính hợp lệ của dữ liệu
		this.addupdate(this.CreateData());
	}
  addupdate(data:DemoModel){
    this._TestPageService.addupdate(data).subscribe(res => {
			this.activeModal.close(res);
		});
  }
  CreateData(): DemoModel {
    const controls = this.Form.controls;
    const _tmp = new DemoModel();
    _tmp.name = controls['Name'].value;
    if (this._data.id > 0) {
      _tmp.id = this._data.id;
    }
    return _tmp;
  }
  GetTitle() {
    if (this._data.id > 0) {
      return "Chỉnh sửa";
    }
    return "Thêm mới";
  }
  public dismiss(): void {
    if (this.activeModal)
      this.activeModal.dismiss();
  }
}
