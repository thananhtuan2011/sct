import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { TradePromotionActivityReportPageRoutingModule } from './trade-promotion-activity-report-page-routing.module';
import { TradePromotionActivityReportPageComponent } from './trade-promotion-activity-report-page.component';
import { TradePromotionActivityReportService } from './_services/trade-promotion-activity-report-page.service';
import { TableTradePromotionActivityReportPageComponent } from './table-trade-promotion-activity-report-page/table-page.component';
import { EditTradePromotionActivityReportModalComponent } from './table-trade-promotion-activity-report-page/components/edit-trade-promotion-activity-report-modal/edit-modal.component';
import { AddEnterpriseInProvinceModalComponent } from './table-trade-promotion-activity-report-page/components/edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from './table-trade-promotion-activity-report-page/components/edit-enterprises-outside-province-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';
import { MatRadioModule } from '@angular/material/radio';

@NgModule({
  declarations: [
    TradePromotionActivityReportPageComponent,
    TableTradePromotionActivityReportPageComponent,
    EditTradePromotionActivityReportModalComponent,
    AddEnterpriseInProvinceModalComponent,
    AddEnterpriseOutsideProvinceModalComponent
  ],
  providers: [
    TradePromotionActivityReportService,
	],
  imports: [
    CommonModule,
    TradePromotionActivityReportPageRoutingModule,
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
    MatButtonModule,
    SearchPipeModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule,
    MatRadioModule
  ]
})
export class TradePromotionActivityReportPageModule {}