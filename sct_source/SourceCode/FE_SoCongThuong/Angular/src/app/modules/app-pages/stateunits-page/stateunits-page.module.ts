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
import { StateUnitsPageRoutingModule } from './stateunits-page-routing.module';
import { TableStateUnitsPageComponent } from './table-stateunits-page/table-stateunits-page.component';
import { StateUnitsPageComponent } from './stateunits-page.component';
import { StateUnitsPageService } from './_services/stateunits-page.service';
import { EditStateUnitsModalComponent } from './table-stateunits-page/components/edit-stateunits-modal/edit-stateunits-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    StateUnitsPageComponent,
    TableStateUnitsPageComponent,
    EditStateUnitsModalComponent
  ],
  providers: [
    StateUnitsPageService
	],
  imports: [
    CommonModule,
    StateUnitsPageRoutingModule,
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
export class StateUnitsPageModule {}