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
import { TestGuidManagementPageRoutingModule } from './testguidmanage-page-routing.module';
import { TableTestGuidManagementPageComponent } from './table-testguidmanagement-page/table-page.component';
import { TestGuidManagementPageComponent } from './testguidmanage-page.component';
import { TestGuidManagementPageService } from './_services/testguidmanagement.service';
import { EditTestGuidManagementModalComponent } from './table-testguidmanagement-page/components/edit-testguidmanagement-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TestGuidManagementPageComponent,
    TableTestGuidManagementPageComponent,
    EditTestGuidManagementModalComponent,
  ],
  providers: [
    TestGuidManagementPageService,
	],
  imports: [
    CommonModule,
    TestGuidManagementPageRoutingModule,
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
export class TestGuidManagementPageModule {}