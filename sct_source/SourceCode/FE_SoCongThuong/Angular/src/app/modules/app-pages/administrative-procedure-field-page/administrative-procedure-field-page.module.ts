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
import { AdministrativeProcedureFieldRoutingModule } from './administrative-procedure-field-page-routing.module';
import { TableAdministrativeProcedureFieldComponent } from './table-administrative-procedure-field-page/table-page.component';
import { AdministrativeProcedureFieldComponent } from './administrative-procedure-field-page.component';
import { AdministrativeProcedureFieldService } from './_services/administrative-procedure-field-page.service';
import { EditAdministrativeProcedureFieldModalComponent } from './table-administrative-procedure-field-page/components/edit-administrative-procedure-field-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    AdministrativeProcedureFieldComponent,
    TableAdministrativeProcedureFieldComponent,
    EditAdministrativeProcedureFieldModalComponent
  ],
  providers: [
    AdministrativeProcedureFieldService
	],
  imports: [
    CommonModule,
    AdministrativeProcedureFieldRoutingModule,
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
    ImportDirectModule
  ]
})
export class AdministrativeProcedureFieldModule {}