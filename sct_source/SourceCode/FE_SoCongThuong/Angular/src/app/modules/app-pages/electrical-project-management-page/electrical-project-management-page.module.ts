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

import { ElectricalProjectManagementPageService } from './_services/electrical-project-management-page.service';
import { ElectricalProjectManagementPageRoutingModule } from './electrical-project-management-page-routing.module';

import { ElectricalProjectManagementPageComponent } from './electrical-project-management-page.component';
import { TableElectricalProjectManagementPageComponent } from './table-electrical-project-management-page/table-page.component';
import { EditElectricalProjectManagementModalComponent } from './table-electrical-project-management-page/components/edit-electrical-project-management-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';

@NgModule({
  declarations: [
    ElectricalProjectManagementPageComponent,
    TableElectricalProjectManagementPageComponent,
    EditElectricalProjectManagementModalComponent
  ],
  providers: [
    ElectricalProjectManagementPageService
	],
  imports: [
    CommonModule,
    ElectricalProjectManagementPageRoutingModule,
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
    DatePickerCustomModule
  ]
})
export class ElectricalProjectManagementPageModule {}
