import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AdministrativeProceduresPageRoutingModule } from './administrative-procedures-page-routing.module';
import { TableAdministrativeProceduresPageComponent } from './table-administrative-procedures-page/table-page.component';
import { AdministrativeProceduresPageComponent } from './administrative-procedures-page.component';
import { AdministrativeProceduresPageService } from './_services/administrative-procedures-page.service';
import { EditAdministrativeProceduresModalComponent } from './table-administrative-procedures-page/components/edit-administrative-procedures-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';

import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    AdministrativeProceduresPageComponent,
    TableAdministrativeProceduresPageComponent,
    EditAdministrativeProceduresModalComponent
  ],
  providers: [
    AdministrativeProceduresPageService,
	],
  imports: [
    CommonModule,
    AdministrativeProceduresPageRoutingModule,
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
    NgbDropdownModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class AdministrativeProceduresPageModule {}