import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { EditProductionBusinessModalComponent } from '../edit-production-business-modal/edit-modal.component';
import { EditImportExportBusinessModalComponent } from '../edit-import-export-business-modal/edit-modal.component';

@Component({
  selector: 'app-edit-modal',
  templateUrl: './edit-modal.component.html',
  styleUrls: ['./edit-modal.component.scss'],
})
export class EditFinancialPlanTargetsModalComponent implements OnInit {
  @Input() id: any;
  @Input() type: any;
  isLoading$: any;
  showTab0: boolean = true;
  showTab1: boolean = true;

  @ViewChild(EditProductionBusinessModalComponent) ProductionBusinessComponent: EditProductionBusinessModalComponent;
  @ViewChild(EditImportExportBusinessModalComponent) ImportExportBusinessComponent: EditImportExportBusinessModalComponent;

  constructor(
    public modal: NgbActiveModal,
  ) { }

  ngOnInit(): void {
    if (this.id && this.type) {
      if (this.type < 3) {
        this.showTab1 = false
      } else if (this.type > 2 && this.type < 11) {
        this.showTab0 = false
      }
    }
  }

  TabChange(event: MatTabChangeEvent) {
    if (event.index == 0) {
      this.ImportExportBusinessComponent.resetForm();
    } else if (event.index == 1) {
      this.ProductionBusinessComponent.resetForm();
    }
  }
}