import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from '../../../_metronic/shared/crud-table/crud-table.module';
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
import { TrainingClassManagementPageRoutingModule } from './trainclassmanage-page-routing.module';
import { TableTrainingClassManagementPageComponent } from './table-trainingclassmanagement-page/table-page.component';
import { TrainingClassManagementPageComponent } from './trainclassmanage-page.component';
import { TrainingClassManagementPageService } from './_services/trainingclassmanagement.service';
import { EditTrainingClassManagementModalComponent } from './table-trainingclassmanagement-page/components/edit-trainingclassmanagement-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TrainingClassManagementPageComponent,
    TableTrainingClassManagementPageComponent,
    EditTrainingClassManagementModalComponent,
  ],
  providers: [
    TrainingClassManagementPageService,
	],
  imports: [
    CommonModule,
    TrainingClassManagementPageRoutingModule,
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
    MatIconModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})

export class GasTrainingClassManagementPageModule {}