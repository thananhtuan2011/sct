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
import { MatMenuModule } from '@angular/material/menu';

import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'

import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { SysLogsPageRoutingModule } from './sys-logs-routing.module';
import { SysLogsPageService } from './_services/sys-logs-page.service';
import { SysLogsPageComponent } from './sys-logs-page.component';
import { TableSysLogsPageComponent } from './table-sys-logs-page/table-sys-logs-page.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    SysLogsPageComponent,
    TableSysLogsPageComponent
  ],
  providers: [
    SysLogsPageService,
	],
  imports: [
    CommonModule,
    SysLogsPageRoutingModule,
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
export class SysLogsPageModule {}
