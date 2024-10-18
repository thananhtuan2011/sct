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
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ProcessAdministrativeProceduresPageRoutingModule } from './process-administrative-procedures-page-routing.module';
import { TableProcessAdministrativeProceduresPageComponent } from './table-process-administrative-procedures-page/table-page.component';
import { ProcessAdministrativeProceduresPageComponent } from './process-administrative-procedures-page.component';
import { ProcessAdministrativeProceduresPageService } from './_services/process-administrative-procedures-page.service';
import { EditProcessAdministrativeProceduresModalComponent } from './table-process-administrative-procedures-page/components/edit-process-administrative-procedures-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    ProcessAdministrativeProceduresPageComponent,
    TableProcessAdministrativeProceduresPageComponent,
    EditProcessAdministrativeProceduresModalComponent
  ],
  providers: [
    ProcessAdministrativeProceduresPageService
	],
  imports: [
    CommonModule,
    ProcessAdministrativeProceduresPageRoutingModule,
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
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class ProcessAdministrativeProceduresPageModule {}