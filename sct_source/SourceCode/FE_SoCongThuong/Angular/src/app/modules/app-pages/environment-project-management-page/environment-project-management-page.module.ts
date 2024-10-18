import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbDateAdapter, NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';

import { EnvironmentProjectManagementPageRoutingModule } from './environment-project-management-page-routing.module';
import { EnvironmentProjectManagementPageComponent } from './environment-project-management-page.component';
import { EnvironmentProjectManagementPageService } from './_services/environment-project-management-page.service';


import { TableEnvironmentProjectManagementPageComponent } from './table-environment-project-management-page/table-page.component';

import { EditEnvironmentProjectManagementModalComponent } from './table-environment-project-management-page/components/edit-environment-project-management-modal/edit-modal.component';
import { progressModule } from 'src/app/_metronic/shared/components/progress-upload/progress.component.module';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';



@NgModule({
  declarations: [
    EnvironmentProjectManagementPageComponent,
    TableEnvironmentProjectManagementPageComponent,
    EditEnvironmentProjectManagementModalComponent,
  ],
  providers: [
    EnvironmentProjectManagementPageService,
    
    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    DndDirectiveModule,
    progressModule,
    EnvironmentProjectManagementPageRoutingModule,
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
export class EnvironmentProjectManagementPageModule {}