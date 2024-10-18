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
import { DistrictPageRoutingModule } from './district-page-routing.module';
import { TableDistrictPageComponent } from './table-district-page/table-district-page.component';
import { DistrictPageComponent } from './district-page.component';
import { DistrictPageService } from './_services/district-page.service';
import { EditDistrictModalComponent } from './table-district-page/components/edit-district-modal/edit-district-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    DistrictPageComponent,
    TableDistrictPageComponent,
    EditDistrictModalComponent
  ],
  providers: [
    DistrictPageService
	],
  imports: [
    CommonModule,
    DistrictPageRoutingModule,
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
export class DistrictPageModule {}
