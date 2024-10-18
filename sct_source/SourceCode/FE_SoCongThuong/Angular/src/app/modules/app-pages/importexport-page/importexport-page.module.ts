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
import { MatMenuModule } from '@angular/material/menu';


import { ImportExportPageComponent } from './importexport-page.component';
import { ImportExportPageRoutingModule } from './importexport-page-routing.module';

//Service
import { ImportGoodsPageService } from './_services/importgoods-page.service';
import { ExportGoodsPageService } from './_services/exportgoods-page.service';
import { ImportExportInfoPageService } from './_services/importexportinfo-page.service';

//Table Page
import { TableImportGoodsPageComponent } from './table-importgoods-page/table-importgoods-page.component';
import { TableExportGoodsPageComponent } from './table-exportgoods-page/table-exportgoods-page.component';
import { TableImportExportInfoPageComponent } from './table-importexportinfo-page/table-importexportinfo-page.component';

//Edit page
import { EditImportGoodsModalComponent } from './table-importgoods-page/components/edit-importgoods-modal/edit-importgoods-modal.component';
import { EditExportGoodsModalComponent } from './table-exportgoods-page/components/edit-exportgoods-modal/edit-exportgoods-modal.component'

import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'

import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    ImportExportPageComponent,

    TableImportGoodsPageComponent,
    TableExportGoodsPageComponent,
    TableImportExportInfoPageComponent,

    EditImportGoodsModalComponent,
    EditExportGoodsModalComponent,
  ],
  providers: [
    ImportGoodsPageService,
    ExportGoodsPageService,
    ImportExportInfoPageService,

    //Custom NgbDate ouput: Array {day: 'd', month: 'MM', year: 'yyyy'} --> String 'dd/MM/yyyy'
		{ provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    ImportExportPageRoutingModule,
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
export class ImportExportPageModule {}