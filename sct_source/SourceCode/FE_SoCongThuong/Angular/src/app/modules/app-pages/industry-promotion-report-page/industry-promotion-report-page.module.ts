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
import { IndustryPromotionReportPageRoutingModule } from './industry-promotion-report-page-routing.module';
import { TableIndustryPromotionReportPageComponent } from './table-industry-promotion-report-page/table-page.component';
import { IndustryPromotionReportPageComponent } from './industry-promotion-report-page.component';
import { IndustryPromotionReportPageService } from './_services/industry-promotion-report-page.service';
import { EditIndustryPromotionReportModalComponent } from './table-industry-promotion-report-page/components/edit-industry-promotion-report-modal/edit-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    IndustryPromotionReportPageComponent,
    TableIndustryPromotionReportPageComponent,
    EditIndustryPromotionReportModalComponent
  ],
  providers: [
    IndustryPromotionReportPageService
	],
  imports: [
    CommonModule,
    IndustryPromotionReportPageRoutingModule,
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
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule
  ]
})
export class IndustryPromotionReportPageModule {}
