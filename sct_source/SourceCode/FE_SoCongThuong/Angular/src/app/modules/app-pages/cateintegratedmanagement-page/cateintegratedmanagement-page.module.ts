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
import { MatTabsModule } from '@angular/material/tabs';

import { CateIntegratedManagementPageComponent } from './cateintegratedmanagement-page.component';
import { CateIntegratedManagementPageRoutingModule } from './cateintegratedmanagement-page-routing.module';

//Service
import { CateIntegratedManagementPageService } from './_services/cateintegratedmanagement-page.service';

//Table Page
import { TableCateIntegratedManagementPageComponent } from './table-cateintegratedmanagement-page/table-page.component';

//Info page
import { InfoPageComponent } from './table-cateintegratedmanagement-page/components/info-cateintegratedmanagement-modal/info-page.component';


import { CustomAdapter, CustomDateParserFormatter } from '../../../_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    CateIntegratedManagementPageComponent,
    TableCateIntegratedManagementPageComponent,
    InfoPageComponent,
  ],
  providers: [
    CateIntegratedManagementPageService,

    //Custom NgbDate ouput: Array {day: 'd', month: 'MM', year: 'yyyy'} --> String 'dd/MM/yyyy'
		{ provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    CateIntegratedManagementPageRoutingModule,
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
    MatTabsModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class CateIntegratedManagementPageModule {}