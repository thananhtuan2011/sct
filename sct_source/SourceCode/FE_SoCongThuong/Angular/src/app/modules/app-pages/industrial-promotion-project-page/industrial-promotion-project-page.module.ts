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
import { NgbDateAdapter, NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'
import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';

import { IndustrialPromotionProjectPageRoutingModule } from './industrial-promotion-project-page-routing.module';
import { IndustrialPromotionProjectPageComponent } from './industrial-promotion-project-page.component';

import { IndustrialPromotionProjectService } from './_services/industrial-promotion-project-page.service';

import { TableIndustrialPromotionProjectPageComponent } from './table-industrial-promotion-project-page/table-page.component';

import { EditIndustrialPromotionProjectModalComponent } from './table-industrial-promotion-project-page/components/edit-industrial-promotion-project-modal/edit-modal.component';

import { AddEnterpriseInProvinceModalComponent } from './table-industrial-promotion-project-page/components/edit-enterprises-in-province-modal/edit-modal.component';
import { AddEnterpriseOutsideProvinceModalComponent } from './table-industrial-promotion-project-page/components/edit-enterprises-outside-province-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    IndustrialPromotionProjectPageComponent,

    TableIndustrialPromotionProjectPageComponent,

    EditIndustrialPromotionProjectModalComponent,

    AddEnterpriseInProvinceModalComponent,
    AddEnterpriseOutsideProvinceModalComponent
  ],
  providers: [
    IndustrialPromotionProjectService,

    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    IndustrialPromotionProjectPageRoutingModule,
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
    MonthYearPickerCustomModule
  ]
})
export class IndustrialPromotionProjectPageModule {}