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
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';

import { NewRuralCriteriaPageService } from './_services/new-rural-criteria-page.service';
import { NewRuralCriteriaPageRoutingModule } from './new-rural-criteria-page-routing.module';

import { NewRuralCriteriaPageComponent } from './new-rural-criteria-page.component';
import { TableNewRuralCriteriaPageComponent } from './table-new-rural-criteria-page/table-page.component';
import { EditNewRuralCriteriaModalComponent } from './table-new-rural-criteria-page/components/edit-new-rural-criteria-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';


@NgModule({
  declarations: [
    NewRuralCriteriaPageComponent,
    TableNewRuralCriteriaPageComponent,
    EditNewRuralCriteriaModalComponent
  ],
  providers: [
    NewRuralCriteriaPageService
	],
  imports: [
    CommonModule,
    NewRuralCriteriaPageRoutingModule,
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
    MatRadioModule,
    CRUDTableModule,
    SelectCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule
  ]
})
export class NewRuralCriteriaPageModule {}
