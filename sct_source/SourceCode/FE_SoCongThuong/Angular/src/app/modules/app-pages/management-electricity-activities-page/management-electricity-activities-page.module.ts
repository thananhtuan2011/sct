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

import { ManagementElectricityActivitiesPageService } from './_services/management-electricity-activities-page.service';
import { MonthReportPageService } from './_services/month-report-page.service';
import { ManagementElectricityActivitiesPageRoutingModule } from './management-electricity-activities-page-routing.module';

import { ManagementElectricityActivitiesPageComponent } from './management-electricity-activities-page.component';
import { TablePageComponent } from './table-management-electricity-activities-page/table-page.component';
import { TableManagementElectricityActivitiesPageComponent } from './table-management-electricity-activities-page/components/table-managerment-electricity-activities-modal/table-page.component';
import { EditManagementElectricityActivitiesModalComponent } from './table-management-electricity-activities-page/components/edit-management-electricity-activities-modal/edit-modal.component';
import { EditMonthReportModalComponent } from './table-management-electricity-activities-page/components/edit-month-report-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabsModule } from '@angular/material/tabs';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';
import { TableMonthReportPageComponent } from './table-management-electricity-activities-page/components/table-month-report-modal/table-page.component';

@NgModule({
  declarations: [
    ManagementElectricityActivitiesPageComponent,
    TablePageComponent,
    TableManagementElectricityActivitiesPageComponent,
    TableMonthReportPageComponent,
    EditManagementElectricityActivitiesModalComponent,
    EditMonthReportModalComponent,
  ],
  providers: [
    ManagementElectricityActivitiesPageService,
    MonthReportPageService
	],
  imports: [
    CommonModule,
    ManagementElectricityActivitiesPageRoutingModule,
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
    MatCheckboxModule,
    CRUDTableModule,
    SelectCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule,
    DateTimePickerModule,
    MatRadioModule,
    MatTabsModule,
    NgxDropzoneModule,
    DndDirectiveModule,
    MonthYearPickerCustomModule
  ]
})
export class ManagementElectricityActivitiesPageModule {}
