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
import { TradePromotionProjectManagementPageRoutingModule } from './tradepromotionPM-page-routing.module';
import { TableTradePromotionProjectManagementPageComponent } from './table-tradepromotionPM-page/table-tradepromotionPM-page.component';
import { TradePromotionProjectManagementPageComponent } from './tradepromotionPM-page.component';
import { TradePromotionProjectManagementPageService } from './_services/tradepromotionPM-page.service';
import { EditTradePromotionProjectModalComponent } from './table-tradepromotionPM-page/components/edit-tradepromotionprojectmanagement-modal/edit-tradepromotionPM-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { AddEnterpriseInProvinceModalComponent } from './table-tradepromotionPM-page/components/edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from './table-tradepromotionPM-page/components/edit-enterprises-outside-province-modal/edit-modal.component';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    TradePromotionProjectManagementPageComponent,
    TableTradePromotionProjectManagementPageComponent,
    EditTradePromotionProjectModalComponent,
    AddEnterpriseOutsideProvinceModalComponent,
    AddEnterpriseInProvinceModalComponent
  ],
  providers: [
    TradePromotionProjectManagementPageService,
    // { provide: NgbDateAdapter, useClass: CustomAdapter },
		// { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    TradePromotionProjectManagementPageRoutingModule,
    InlineSVGModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
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
export class TradePromotionProjectManagementPageModule {}