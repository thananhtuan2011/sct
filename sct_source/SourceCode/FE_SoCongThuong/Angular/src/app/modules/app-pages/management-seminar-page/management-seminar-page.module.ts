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

import { ManagementSeminarPageRoutingModule } from './management-seminar-page-routing.module';
import { ManagementSeminarPageComponent } from './management-seminar-page.component';


import { ManagementSeminarService } from './_services/management-seminar-page.service';


import { TableManagementSeminarPageComponent } from './table-management-seminar-page/table-page.component';


import { EditManagementSeminarModalComponent } from './table-management-seminar-page/components/management-seminar-modal/edit-modal.component';

import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';


@NgModule({
  declarations: [
    ManagementSeminarPageComponent,
    TableManagementSeminarPageComponent,
    EditManagementSeminarModalComponent,

  ],
  providers: [
    ManagementSeminarService,

    //Chỉ dùng khi có ngày không
    // { provide: NgbDateAdapter, useClass: CustomAdapter },
		// { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    ManagementSeminarPageRoutingModule,
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
    DateTimePickerModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class ManagementSeminarPageModule {}