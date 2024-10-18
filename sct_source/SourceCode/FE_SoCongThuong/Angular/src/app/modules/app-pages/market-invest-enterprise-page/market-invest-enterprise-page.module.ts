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
import { MarketInvestEnterprisePageComponent } from './market-invest-enterprise-page.component';
import { MarketInvestEnterprisePageRoutingModule } from './market-invest-enterprise-page-routing.module';
import { TableMarketInvestEnterprisePageComponent } from './table-market-invest-enterprise-page/table-page.component';
import { MarketInvestEnterprisePageService } from './_services/market-invest-enterprise-page.service';
import { EditMarketInvestEnterpriseModalComponent } from './table-market-invest-enterprise-page/components/edit-market-invest-enterprise-modal/edit-modal.component';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    MarketInvestEnterprisePageComponent,
    TableMarketInvestEnterprisePageComponent,
    EditMarketInvestEnterpriseModalComponent,
  ],
  providers: [
    MarketInvestEnterprisePageService
	],
  imports: [
    MarketInvestEnterprisePageRoutingModule,
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
export class MarketInvestEnterprisePageModule {}