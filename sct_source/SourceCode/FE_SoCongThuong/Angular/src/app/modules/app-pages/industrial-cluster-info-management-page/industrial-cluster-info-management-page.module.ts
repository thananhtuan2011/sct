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
import { IndustrialClusterInfoManagementPageRoutingModule } from './industrial-cluster-info-management-page-routing.module';
import { IndustrialClusterInfoManagementPageComponent } from './industrial-cluster-info-management-page.component';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';

import { CateInvestmentProjectPageService } from './_services/cate-investment-project-page.service';
import { CateIndustrialClusterPageService } from './_services/cate-industrial-cluster-page.service';

import { TableCateInvestmentProjectPageComponent } from './table-cate-investment-project-page/table-page.component';
import { TableCateIndustrialClusterPageComponent } from './table-cate-industrial-cluster-page/table-page.component';

import { EditCateInvestmentProjectModalComponent } from './table-cate-investment-project-page/components/edit-cate-investment-project-modal/edit-modal.component';
import { EditCateIndustrialClusterModalComponent } from './table-cate-industrial-cluster-page/components/edit-cate-industrial-cluster-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';


@NgModule({
  declarations: [
    IndustrialClusterInfoManagementPageComponent,

    TableCateInvestmentProjectPageComponent,
    TableCateIndustrialClusterPageComponent,

    EditCateInvestmentProjectModalComponent,
    EditCateIndustrialClusterModalComponent,
  
  ],
  providers: [
    CateInvestmentProjectPageService,
    CateIndustrialClusterPageService,

    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    IndustrialClusterInfoManagementPageRoutingModule,
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
    ImportDirectModule
  ]
})
export class IndustrialClusterInfoManagementPageModule {}