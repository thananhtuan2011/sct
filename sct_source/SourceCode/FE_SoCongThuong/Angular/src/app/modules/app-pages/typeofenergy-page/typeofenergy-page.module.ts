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
import { TypeOfEnergyPageRoutingModule } from './typeofenergy-page-routing.module';
import { TableTypeOfEnergyPageComponent } from './table-typeofenergy-page/table-typeofenergy-page.component';
import { TypeOfEnergyPageComponent } from './typeofenergy-page.component';
import { TypeOfEnergyPageService } from './_services/typeofenergy-page.service';
import { EditTypeOfEnergyModalComponent } from './table-typeofenergy-page/components/edit-typeofenergy-modal/edit-typeofenergy-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TypeOfEnergyPageComponent,
    TableTypeOfEnergyPageComponent,
    EditTypeOfEnergyModalComponent
  ],
  providers: [
    TypeOfEnergyPageService
	],
  imports: [
    CommonModule,
    TypeOfEnergyPageRoutingModule,
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
export class TypeOfEnergyPageModule {}