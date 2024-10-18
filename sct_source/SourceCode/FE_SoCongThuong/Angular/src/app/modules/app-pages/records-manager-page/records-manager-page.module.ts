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
import { RecordsManagerPageRoutingModule } from './records-manager-page-routing.module';
import { TableRecordsManagerPageComponent } from './table-records-manager-page/table-records-manager-page.component';
import { RecordsManagerPageComponent } from './records-manager-page.component';
import { RecordsManagerPageService } from './_services/records-manager-page.service';
import { EditRecordsManagerModalComponent } from './table-records-manager-page/components/edit-records-manager-modal/edit-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    RecordsManagerPageComponent,
    TableRecordsManagerPageComponent,
    EditRecordsManagerModalComponent
  ],
  providers: [
    RecordsManagerPageService
	],
  imports: [
    CommonModule,
    RecordsManagerPageRoutingModule,
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
    ImportDirectModule,
    SelectCustomModule,
    DatePickerCustomModule,
    MatIconModule,
    MatButtonModule
  ]
})
export class RecordsManagerPageModule {}
