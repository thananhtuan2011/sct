import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectCustomModule } from '../../../_metronic/shared/components/select-custom/select-custom.module';
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
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from '../../../_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'
import { SearchPipeModule } from '../../../_metronic/shared/pipe/filter-pipe/filter.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';

import { IndustrialManagementTargetPageRoutingModule } from './target-page-routing.module';
import { IndustrialManagementTargetPageComponent } from './target-page.component';

import { IndustrialManagementTargetPageService } from './_services/indus-manage-target.service';

import { TableIndustrialManagementTargetPageComponent } from './table-indus-manage-target-page/table-page.component';

import { EditIndustrialManagementTargetModalComponent } from './table-indus-manage-target-page/components/edit-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from '../../../_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from '../../../_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    IndustrialManagementTargetPageComponent,
    TableIndustrialManagementTargetPageComponent,
    EditIndustrialManagementTargetModalComponent,
  ],
  providers: [
    IndustrialManagementTargetPageService,
	],
  imports: [
    CommonModule,
    IndustrialManagementTargetPageRoutingModule,
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
    ImportDirectModule
  ]
})
export class IndustrialManagementTargetPageModule {}