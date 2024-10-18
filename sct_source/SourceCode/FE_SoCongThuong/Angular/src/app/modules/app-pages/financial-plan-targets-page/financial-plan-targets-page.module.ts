import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';

//Material module
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatRadioModule } from '@angular/material/radio';

//Directive module
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

//Page Component
import { FinancialPlanTargetsPageComponent } from './financial-plan-targets-page.component';
import { FinancialPlanTargetsPageRoutingModule } from './financial-plan-targets-page-routing.module';
import { TableFinancialPlanTargetsPageComponent } from './table-financial-plan-targets-page/table-page.component';
import { FinancialPlanTargetsPageService } from './_services/financial-plan-targets-page.service';
import { EditFinancialPlanTargetsModalComponent } from './table-financial-plan-targets-page/components/edit-financial-plan-targets-modal/edit-modal.component';
import { EditProductionBusinessModalComponent } from './table-financial-plan-targets-page/components/edit-production-business-modal/edit-modal.component';
import { EditImportExportBusinessModalComponent } from './table-financial-plan-targets-page/components/edit-import-export-business-modal/edit-modal.component';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    FinancialPlanTargetsPageComponent,
    TableFinancialPlanTargetsPageComponent,
    EditFinancialPlanTargetsModalComponent,
    EditProductionBusinessModalComponent,
    EditImportExportBusinessModalComponent
  ],
  providers: [
    FinancialPlanTargetsPageService
	],
  imports: [
    FinancialPlanTargetsPageRoutingModule,
    CommonModule,
    InlineSVGModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    CRUDTableModule,
    SelectCustomModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatMenuModule,
    MatTabsModule,
    MatRadioModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule,
    MonthYearPickerCustomModule
  ]
})
export class FinancialPlanTargetsPageModule {}