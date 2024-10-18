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

import { ElectricOperatingUnitsPageService } from './_services/electric-operating-units-page.service';
import { ElectricOperatingUnitsPageRoutingModule } from './electric-operating-units-page-routing.module';

import { ElectricOperatingUnitsPageComponent } from './electric-operating-units-page.component';
import { TableElectricOperatingUnitsPageComponent } from './table-electric-operating-units-page/table-page.component';
import { EditElectricOperatingUnitsModalComponent } from './table-electric-operating-units-page/components/edit-electric-operating-units-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { MatRadioModule } from '@angular/material/radio';

@NgModule({
  declarations: [
    ElectricOperatingUnitsPageComponent,
    TableElectricOperatingUnitsPageComponent,
    EditElectricOperatingUnitsModalComponent
  ],
  providers: [
    ElectricOperatingUnitsPageService
	],
  imports: [
    CommonModule,
    ElectricOperatingUnitsPageRoutingModule,
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
    MatCheckboxModule,
    CRUDTableModule,
    SelectCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule,
    DateTimePickerModule,
    MatRadioModule
  ]
})
export class ElectricOperatingUnitsPageModule {}
