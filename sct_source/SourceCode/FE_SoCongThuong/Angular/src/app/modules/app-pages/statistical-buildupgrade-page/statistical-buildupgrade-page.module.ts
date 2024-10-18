import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { LOCALE_ID, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { NgbDateAdapter, NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MatNativeDateModule } from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';

import { MatListModule } from '@angular/material/list';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatMomentDateModule } from "@angular/material-moment-adapter";
import { MatTabsModule } from '@angular/material/tabs';

import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate'
import { NgxDropzoneModule } from 'ngx-dropzone';

import { StatisticalBuildUpgradePageRoutingModule } from './statistical-buildupgrade-page-routing.module';
import { TableStatisticalBuildUpgradePageComponent } from './table-statistical-buildupgrade-page/table-page.component';
import { StatisticalBuildUpgradePageComponent } from './statistical-buildupgrade-page.component';
import { StatisticalBuildUpgradePageService } from './_services/statistical-buildupgrade-page.service';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { SearchPipe } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.pipe';

@NgModule({
  declarations: [
    StatisticalBuildUpgradePageComponent,
    TableStatisticalBuildUpgradePageComponent,
  ],
  providers: [
    StatisticalBuildUpgradePageService,
    MatNativeDateModule,
    SearchPipe,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  
  imports: [
    CommonModule,
    StatisticalBuildUpgradePageRoutingModule,
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
    NgxDropzoneModule,
    MatTabsModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class StatisticalBuildUpgradePageModule {}