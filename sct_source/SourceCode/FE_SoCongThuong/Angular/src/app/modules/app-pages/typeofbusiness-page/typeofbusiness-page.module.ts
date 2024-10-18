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
import { TypeOfBusinessPageRoutingModule } from './typeofbusiness-page-routing.module';
import { TableTypeOfBusinessPageComponent } from './table-typeofbusiness-page/table-typeofbusiness-page.component';
import { TypeOfBusinessPageComponent } from './typeofbusiness-page.component';
import { TypeOfBusinessPageService } from './_services/typeofbusiness-page.service';
import { EditTypeOfBusinessModalComponent } from './table-typeofbusiness-page/components/edit-typeofbusiness-modal/edit-typeofbusiness-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TypeOfBusinessPageComponent,
    TableTypeOfBusinessPageComponent,
    EditTypeOfBusinessModalComponent
  ],
  providers: [
    TypeOfBusinessPageService
	],
  imports: [
    CommonModule,
    TypeOfBusinessPageRoutingModule,
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
export class TypeOfBusinessPageModule {}