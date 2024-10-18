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

import { EditCommuneElectricityManagementPageService } from './_services/commune-electricity-management-page.service';
import { CommuneElectricityManagementPageRoutingModule } from './electricity-management-page-routing.module';

import { CommuneElectricityManagementPageComponent } from './electricity-management-page.component';
import { TableCommuneElectricityManagementPageComponent } from './table-electricity-management-page/table-page.component';
import { EditCommuneElectricityManagementModalComponent } from './table-electricity-management-page/components/edit-electricity-management-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    CommuneElectricityManagementPageComponent,
    TableCommuneElectricityManagementPageComponent,
    EditCommuneElectricityManagementModalComponent
  ],
  providers: [
    EditCommuneElectricityManagementPageService
	],
  imports: [
    CommonModule,
    CommuneElectricityManagementPageRoutingModule,
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
    ImportDirectModule
  ]
})
export class CommuneElectricityManagementPageModule {}
