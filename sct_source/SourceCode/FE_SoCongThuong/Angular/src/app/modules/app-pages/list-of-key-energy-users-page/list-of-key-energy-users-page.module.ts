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

import { ListOfKeyEnergyUsersPageService } from './_services/list-of-key-energy-users-page.service';
import { ListOfKeyEnergyUsersPageRoutingModule } from './list-of-key-energy-users-page-routing.module';

import { ListOfKeyEnergyUsersPageComponent } from './list-of-key-energy-users-page.component';
import { TableListOfKeyEnergyUsersPageComponent } from './table-list-of-key-energy-users-page/table-page.component';
import { EditListOfKeyEnergyUsersModalComponent } from './table-list-of-key-energy-users-page/components/edit-list-of-key-energy-users-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    ListOfKeyEnergyUsersPageComponent,
    TableListOfKeyEnergyUsersPageComponent,
    EditListOfKeyEnergyUsersModalComponent
  ],
  providers: [
    ListOfKeyEnergyUsersPageService
	],
  imports: [
    CommonModule,
    ListOfKeyEnergyUsersPageRoutingModule,
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
    MonthYearPickerCustomModule
  ]
})
export class ListOfKeyEnergyUsersPageModule {}
