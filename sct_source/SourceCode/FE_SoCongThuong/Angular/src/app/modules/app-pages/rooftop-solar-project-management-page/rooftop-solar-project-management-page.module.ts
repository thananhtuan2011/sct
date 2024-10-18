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
import { RooftopSolarProjectManagementPageRoutingModule } from './rooftop-solar-project-management-page-routing.module';
import { TableRooftopSolarProjectManagementPageComponent } from './table-rooftop-solar-project-management-page/table-page.component';
import { RooftopSolarProjectManagementPageComponent } from './rooftop-solar-project-management-page.component';
import { RooftopSolarProjectManagementPageService } from './_services/rooftop-solar-project-management-page.service';
import { EditRooftopSolarProjectManagementModalComponent } from './table-rooftop-solar-project-management-page/components/edit-rooftop-solar-project-management-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';

@NgModule({
  declarations: [
    RooftopSolarProjectManagementPageComponent,
    TableRooftopSolarProjectManagementPageComponent,
    EditRooftopSolarProjectManagementModalComponent
  ],
  providers: [
    RooftopSolarProjectManagementPageService
	],
  imports: [
    CommonModule,
    RooftopSolarProjectManagementPageRoutingModule,
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
    ImportDirectModule,
    DatePickerCustomModule
  ]
})
export class RooftopSolarProjectManagementPageModule {}