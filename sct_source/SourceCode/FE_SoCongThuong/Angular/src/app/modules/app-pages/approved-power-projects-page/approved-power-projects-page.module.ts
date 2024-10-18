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
import { ApprovedPowerProjectsPageRoutingModule } from './approved-power-projects-page-routing.module';
import { TableApprovedPowerProjectsPageComponent } from './table-approved-power-projects-page/table-page.component';
import { ApprovedPowerProjectsPageComponent } from './approved-power-projects-page.component';
import { ApprovedPowerProjectsPageService } from './_services/approved-power-projects-page.service';
import { EditApprovedPowerProjectsModalComponent } from './table-approved-power-projects-page/components/edit-approved-power-projects-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    ApprovedPowerProjectsPageComponent,
    TableApprovedPowerProjectsPageComponent,
    EditApprovedPowerProjectsModalComponent
  ],
  providers: [
    ApprovedPowerProjectsPageService
	],
  imports: [
    CommonModule,
    ApprovedPowerProjectsPageRoutingModule,
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
export class ApprovedPowerProjectsPageModule {}