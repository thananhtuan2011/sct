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

import { MnFvsPProducingAlcoholPageComponent } from './mnfvspproducingalcohol-page.component';
import { MnFvsPProducingAlcoholPageRoutingModule } from './mnfvspproducingalcohol-page-routing.module';

//Service
import { CateRPSAncolForFactoryPageService } from './_services/caterpsancolforfactory-page.service';
import { CateRPSoldAncolPageService } from './_services/caterpsoldancol-page.service';
import { CateRPProduceIndustlAncolPageService } from './_services/caterpproduceindustlancol-page.service';
import { CateRPTurnOverIndustAncolPageService } from './_services/caterpturnoverindustancol-page.service';
import { CateRPPCrafttAncolForEconomicPageService } from './_services/caterppcrafttancolforeconomic-page.service';

//Table
import { TableCateRPSAncolForFactoryPageComponent } from './table-caterpsancolforfactory-page/table-caterpsancolforfactory-page.component';
import { TableCateRPSoldAncolPageComponent } from './table-caterpsoldancol-page/table-caterpsoldancol-page.component';
import { TableCateRPProduceIndustlAncolPageComponent } from './table-caterpproduceindustlancol-page/table-caterpproduceindustlancol-page.component';
import { TableCateRPTurnOverIndustAncolPageComponent } from './table-caterpturnoverindustancol-page/table-caterpturnoverindustancol-page.component';
import { TableCateRPPCrafttAncolForEconomicPageComponent } from './table-caterppcrafttancolforeconomic-page/table-page.component';

//Edit
import { EditCateRPSAncolForFactoryModalComponent } from './table-caterpsancolforfactory-page/components/edit-caterpsancolforfactory-modal/edit-caterpsancolforfactory-modal.component';
import { EditCateRPSoldAncolModalComponent } from './table-caterpsoldancol-page/components/edit-caterpsoldancol-modal/edit-caterpsoldancol-modal.component';
import { EditCateRPProduceIndustlAncolModalComponent } from './table-caterpproduceindustlancol-page/components/edit-caterpproduceindustlancol-modal/edit-caterpproduceindustlancol-modal.component';
import { EditCateRPTurnOverIndustAncolModalComponent } from './table-caterpturnoverindustancol-page/components/edit-caterpturnoverindustancol-modal/edit-caterpturnoverindustancol-modal.component';
import { EditCateRPPCrafttAncolForEconomicModalComponent } from './table-caterppcrafttancolforeconomic-page/components/edit-caterppcrafttancolforeconomic-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';


@NgModule({
  declarations: [
    MnFvsPProducingAlcoholPageComponent,

    TableCateRPSAncolForFactoryPageComponent,
    TableCateRPPCrafttAncolForEconomicPageComponent,
    TableCateRPSoldAncolPageComponent,
    TableCateRPProduceIndustlAncolPageComponent,
    TableCateRPTurnOverIndustAncolPageComponent,

    EditCateRPSAncolForFactoryModalComponent,
    EditCateRPPCrafttAncolForEconomicModalComponent,
    EditCateRPSoldAncolModalComponent,
    EditCateRPProduceIndustlAncolModalComponent,
    EditCateRPTurnOverIndustAncolModalComponent,
  ],
  providers: [
    CateRPSAncolForFactoryPageService,
    CateRPPCrafttAncolForEconomicPageService,
    CateRPSoldAncolPageService,
    CateRPProduceIndustlAncolPageService,
    CateRPTurnOverIndustAncolPageService,

    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    MnFvsPProducingAlcoholPageRoutingModule,
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
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule
  ]
})
export class MnFvsPProducingAlcoholPageModule {}