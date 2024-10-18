import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgbModule, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';

import { RuralDevelopmentPlanPageRoutingModule } from './rural-development-plan-page-routing.module';
import { RuralDevelopmentPlanPageComponent } from './rural-development-plan-page.component';
//Page Service
import { RuralDevelopmentPlanPageService } from './_services/rural-development-plan-page.service';
//TableComponent
import { TableRuralDevelopmentPlanPageComponent } from './table-rural-development-plan-page/table-page.component';
//Edit Component
import { EditRuralDevelopmentPlanModalComponent } from './table-rural-development-plan-page/components/edit-rural-development-plan-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    RuralDevelopmentPlanPageComponent,
    TableRuralDevelopmentPlanPageComponent,
    EditRuralDevelopmentPlanModalComponent,
  ],
  providers: [
    RuralDevelopmentPlanPageService,
	],
  imports: [
    CommonModule,
    RuralDevelopmentPlanPageRoutingModule,
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
    MatCheckboxModule,
    MatIconModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class RuralDevelopmentPlanPageModule {}