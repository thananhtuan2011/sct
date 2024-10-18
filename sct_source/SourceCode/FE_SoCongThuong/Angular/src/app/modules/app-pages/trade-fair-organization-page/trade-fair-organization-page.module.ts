import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
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
import { MatMenuModule } from '@angular/material/menu';
import { progressModule } from 'src/app/_metronic/shared/components/progress-upload/progress.component.module';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
import { DateTimePickerModule } from '../../../_metronic/shared/components/date-time-picker/date-time-picker.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';

import { TradeFairOrganizationPageRoutingModule } from './trade-fair-organization-page-routing.module';

import { TradeFairOrganizationPageComponent } from './trade-fair-organization-page.component';

import { TradeFairOrganizationCertificationPageService } from './_services/trade-fair-organization-page.service';

import { TableTradeFairOrganizationCertificationPageComponent } from './table-trade-fair-organization-page/table-page.component';

import { EditTradeFairOrganizationCertificationModalComponent } from './table-trade-fair-organization-page/components/edit-trade-fair-organization-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';




@NgModule({
  declarations: [
    TradeFairOrganizationPageComponent,
    TableTradeFairOrganizationCertificationPageComponent,
    EditTradeFairOrganizationCertificationModalComponent,
  ],
  providers: [
    TradeFairOrganizationCertificationPageService,
	],
  imports: [
    CommonModule,
    DndDirectiveModule,
    progressModule,
    TradeFairOrganizationPageRoutingModule,
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
    DateTimePickerModule,
    DatePickerCustomModule,
    MatIconModule,
    MatButtonModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class TradeFairOrganizationPageModule {}