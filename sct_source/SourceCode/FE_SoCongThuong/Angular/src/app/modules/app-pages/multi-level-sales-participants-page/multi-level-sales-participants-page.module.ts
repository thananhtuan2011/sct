import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
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
import { MultiLevelSalesParticipantsPageRoutingModule } from './multi-level-sales-participants-page-routing.module';
import { TableMultiLevelSalesParticipantsPageComponent } from './table-multi-level-sales-participants-page/table-page.component';
import { MultiLevelSalesParticipantsPageComponent } from './multi-level-sales-participants-page.component';
import { MultiLevelSalesParticipantsPageService } from './_services/multi-level-sales-participants-page.service';
import { EditMultiLevelSalesParticipantsModalComponent } from './table-multi-level-sales-participants-page/components/edit-multi-level-sales-participants-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from '../../../_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { ViewReportMultiLevelSalesParticipantsModalComponent } from './table-multi-level-sales-participants-page/components/view-report/view-report-modal.component';

@NgModule({
  declarations: [
    MultiLevelSalesParticipantsPageComponent,
    TableMultiLevelSalesParticipantsPageComponent,
    EditMultiLevelSalesParticipantsModalComponent,
    ViewReportMultiLevelSalesParticipantsModalComponent
  ],
  providers: [
    MultiLevelSalesParticipantsPageService,

    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    MultiLevelSalesParticipantsPageRoutingModule,
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
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class MultiLevelSalesParticipantsPageModule {}