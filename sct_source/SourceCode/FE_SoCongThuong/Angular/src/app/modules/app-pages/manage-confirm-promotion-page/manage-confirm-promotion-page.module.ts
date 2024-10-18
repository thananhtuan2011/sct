import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from '../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbDateAdapter, NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ManageConfirmPromotionPageRoutingModule } from './manage-confirm-promotion-page-routing.module';
import { TableManageConfirmPromotionPageComponent } from './table-manageconfirmpromotion-page/table-manage-confirm-promotion-page.component';
import { ManageConfirmPromotionPageComponent } from './manage-confirm-promotion-page.component';
import { ManageConfirmPromotionPageService } from './_services/manage-confirm-promotion-page.service';
import { EditManageConfirmPromotionModalComponent } from './table-manageconfirmpromotion-page/components/edit-manage-confirm-promotion-page-modal/edit-manage-confirm-promotion-page.component';
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    ManageConfirmPromotionPageComponent,
    TableManageConfirmPromotionPageComponent,
    EditManageConfirmPromotionModalComponent,
  ],
  providers: [
    ManageConfirmPromotionPageService,
    // { provide: NgbDateAdapter, useClass: CustomAdapter },
		// { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    ManageConfirmPromotionPageRoutingModule,
    InlineSVGModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatMenuModule,
    CRUDTableModule,
    SelectCustomModule,
    MatIconModule,
    DateTimePickerModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class ManageConfirmPromotionPageModule {}