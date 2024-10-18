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
import { CateManageAncolLocalBussinesPageRoutingModule } from './catemanageancollocalbussines-page-routing.module';
import { TableCateManageAncolLocalBussinesPageComponent } from './table-catemanageancollocalbussines-page/table-page.component';
import { CateManageAncolLocalBussinesPageComponent } from './catemanageancollocalbussines-page.component';
import { CateManageAncolLocalBussinesPageService } from './_services/catemanageancollocalbussines-page.service';
import { EditCateManageAncolLocalBussinesModalComponent } from './table-catemanageancollocalbussines-page/components/edit-catemanageancollocalbussines-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';

@NgModule({
  declarations: [
    CateManageAncolLocalBussinesPageComponent,
    TableCateManageAncolLocalBussinesPageComponent,
    EditCateManageAncolLocalBussinesModalComponent
  ],
  providers: [
    CateManageAncolLocalBussinesPageService,
    // { provide: NgbDateAdapter, useClass: CustomAdapter },
		// { provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    CateManageAncolLocalBussinesPageRoutingModule,
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
    MatRadioModule,
    MatIconModule,
    MatButtonModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule,
  ]
})
export class CateManageAncolLocalBussinesPageModule {}