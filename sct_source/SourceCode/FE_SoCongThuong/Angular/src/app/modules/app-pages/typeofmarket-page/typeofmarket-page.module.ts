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
import { TypeOfMarketPageRoutingModule } from './typeofmarket-page-routing.module';
import { TableTypeOfMarketPageComponent } from './table-typeofmarket-page/table-typeofmarket-page.component';
import { TypeOfMarketPageComponent } from './typeofmarket-page.component';
import { TypeOfMarketPageService } from './_services/typeofmarket-page.service';
import { EditTypeOfMarketModalComponent } from './table-typeofmarket-page/components/edit-typeofmarket-modal/edit-typeofmarket-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TypeOfMarketPageComponent,
    TableTypeOfMarketPageComponent,
    EditTypeOfMarketModalComponent
  ],
  providers: [
    TypeOfMarketPageService
	],
  imports: [
    CommonModule,
    TypeOfMarketPageRoutingModule,
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
export class TypeOfMarketPageModule {}