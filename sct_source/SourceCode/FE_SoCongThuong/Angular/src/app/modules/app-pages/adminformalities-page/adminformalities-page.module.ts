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
import { AdminFormalitiesPageRoutingModule } from './adminformalities-page-routing.module';
import { TableAdminFormalitiesPageComponent } from './table-adminformalities-page/table-adminformalities-page.component';
import { AdminFormalitiesPageComponent } from './adminformalities-page.component';
import { AdminFormalitiesPageService } from './_services/adminformalities-page.service';
import { EditAdminFormalitiesModalComponent } from './table-adminformalities-page/components/edit-adminformalities-modal/edit-adminformalities-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    AdminFormalitiesPageComponent,
    TableAdminFormalitiesPageComponent,
    EditAdminFormalitiesModalComponent,
  ],
  providers: [
    AdminFormalitiesPageService
	],
  imports: [
    CommonModule,
    AdminFormalitiesPageRoutingModule,
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
export class AdminFormalitiesPageModule {}