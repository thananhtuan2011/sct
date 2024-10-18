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
import { MatMenuModule } from '@angular/material/menu';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'
import { BusinessMultiLevelPageRoutingModule } from './business-multi-level-page-routing.module';
import { TableBusinessMultiLevelPageComponent } from './table-business-multi-level-page/table-page.component';
import { BusinessMultiLevelPageComponent } from './business-multi-level-page.component';
import { BusinessMultiLevelPageService } from './_services/business-multi-level-page.service';
import { EditBusinessMultiLevelModalComponent } from './table-business-multi-level-page/components/edit-business-multi-level-modal/edit-modal.component';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    BusinessMultiLevelPageComponent,
    TableBusinessMultiLevelPageComponent,
    EditBusinessMultiLevelModalComponent
  ],
  providers: [
    BusinessMultiLevelPageService,

    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    BusinessMultiLevelPageRoutingModule,
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
    MatDatepickerModule,
    MatNativeDateModule,
    MatMomentDateModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule
  ]
})
export class BusinessMultiLevelPageModule {}