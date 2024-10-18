import { CRUDTableModule } from '../../../_metronic/shared/crud-table/crud-table.module';
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
import { RecordsFinancePlanPageRoutingModule } from './records-finance-plan-page-routing.module';
import { TableRecordsFinancePlanPageComponent } from './table-records-finance-plan-page/table-records-finance-plan-page.component';
import { RecordsFinancePlanPageComponent } from './records-finance-plan-page.component';
import { RecordsFinancePlanPageService } from './_services/records-finance-plan-page.service';
import { EditRecordsFinancePlanModalComponent } from './table-records-finance-plan-page/components/edit-records-finance-plan-modal/edit-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    RecordsFinancePlanPageComponent,
    TableRecordsFinancePlanPageComponent,
    EditRecordsFinancePlanModalComponent
  ],
  providers: [
    RecordsFinancePlanPageService
	],
  imports: [
    CommonModule,
    RecordsFinancePlanPageRoutingModule,
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
export class RecordsFinancePlanPageModule {}
