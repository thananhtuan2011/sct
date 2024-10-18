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
import { CustomPageRoutingModule } from './custom-page-routing.module';
import { TableCustomPageComponent } from './table-custom-page/table-custom-page.component';
import { CustomPageComponent } from './custom-page.component';
import { CustomPageService } from './_services/custom-page.service';
import { EditCustomModalComponent } from './table-custom-page/components/edit-custom-modal/edit-custom-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    CustomPageComponent,
    TableCustomPageComponent,
    EditCustomModalComponent
  ],
  providers: [
    CustomPageService
	],
  imports: [
    CommonModule,
    CustomPageRoutingModule,
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
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class CustomPageModule {}
