import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ScrollingModule } from '@angular/cdk/scrolling';

import { MatNativeDateModule } from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatListModule } from '@angular/material/list';

import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatMomentDateModule } from "@angular/material-moment-adapter";

import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';

import { StatisticalMultiLevelSalesPageRoutingModule } from './statistical-multi-level-sales-page-routing.module';
import { TableStatisticalMultiLevelSalesPageComponent } from './table-statistical-multi-level-sales-page/table-page.component';
import { StatisticalMultiLevelSalesPageComponent } from './statistical-multi-level-sales-page.component';
import { StatisticalMultiLevelSalesPageService } from './_services/statistical-multi-level-sales-page.service';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
    declarations: [
        StatisticalMultiLevelSalesPageComponent,
        TableStatisticalMultiLevelSalesPageComponent,
    ],
    providers: [
        StatisticalMultiLevelSalesPageService,
    ],

    imports: [
        CommonModule,
        StatisticalMultiLevelSalesPageRoutingModule,
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
        MatListModule,
        ScrollingModule,
        MatDividerModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatToolbarModule,
        MatCheckboxModule,
        SearchPipeModule,
        MatDatepickerModule,
        MatMomentDateModule,
        MatDatepickerModule,
        MatNativeDateModule,
        DatePickerCustomModule,
        NoSpaceAtFirstDirectiveModule,
        MonthYearPickerCustomModule
    ]
})
export class StatisticalMultiLevelSalesPageModule { }