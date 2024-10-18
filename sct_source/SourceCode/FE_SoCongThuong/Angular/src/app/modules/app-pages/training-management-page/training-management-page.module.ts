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

import { TrainingManagementPageRoutingModule } from './training-management-page-routing.module';
import { TrainingManagementPageComponent } from './training-management-page.component';
import { TrainingManagementPageService } from './_services/training-management-page.service';

import { TableTrainingManagementPageComponent } from './table-training-management-page/table-page.component';

import { EditTrainingManagementModalComponent } from './table-training-management-page/components/edit-training-management-modal/edit-modal.component';
import { progressModule } from 'src/app/_metronic/shared/components/progress-upload/progress.component.module';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';



@NgModule({
  declarations: [
    TrainingManagementPageComponent,
    TableTrainingManagementPageComponent,
    EditTrainingManagementModalComponent,
  ],
  providers: [
    TrainingManagementPageService,
	],
  imports: [
    CommonModule,
    DndDirectiveModule,
    progressModule,
    TrainingManagementPageRoutingModule,
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
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule
  ]
})
export class TrainingManagementPageModule {}