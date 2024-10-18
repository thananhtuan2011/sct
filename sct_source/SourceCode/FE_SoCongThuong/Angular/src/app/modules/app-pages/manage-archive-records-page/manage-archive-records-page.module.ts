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
import { ManageArchiveRecordsPageRoutingModule } from './manage-archive-records-page-routing.module';
import { TableManageArchiveRecordsPageComponent } from './table-manage-archive-records-page/table-page.component';
import { ManageArchiveRecordsPageComponent } from './manage-archive-records-page.component';
import { ManageArchiveRecordsPageService } from './_services/manage-archive-records-page.service';
import { EditManageArchiveRecordsModalComponent } from './table-manage-archive-records-page/components/edit-manage-archive-records-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    ManageArchiveRecordsPageComponent,
    TableManageArchiveRecordsPageComponent,
    EditManageArchiveRecordsModalComponent
  ],
  providers: [
    ManageArchiveRecordsPageService
  ],
  imports: [
    CommonModule,
    ManageArchiveRecordsPageRoutingModule,
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
export class ManageArchiveRecordsPageModule { }
