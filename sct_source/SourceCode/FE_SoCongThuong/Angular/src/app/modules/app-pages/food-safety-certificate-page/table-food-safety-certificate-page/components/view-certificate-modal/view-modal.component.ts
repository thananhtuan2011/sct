import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-view-modal',
  templateUrl: './view-modal.component.html',
  styleUrls: ['./view-modal.component.scss'],
})

export class ViewCertificateModalComponent implements OnInit {
  @Input() link: any;
  show: boolean = false;

  constructor (
    public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    if (this.link) {
      this.show = true
    } else {
      this.modal.dismiss();
    }
  }
}
