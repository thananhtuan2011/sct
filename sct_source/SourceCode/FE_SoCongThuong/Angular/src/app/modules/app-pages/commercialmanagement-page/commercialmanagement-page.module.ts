import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgbModule, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { CommercialManagementPageRoutingModule } from './commercialmanagement-page-routing.module';
import { CommercialManagementPageComponent } from './commercialmanagement-page.component';

//Page Service
import { CommercialManagementPageService } from './_services/commercialmanagement-page.service';
import { BuildAndUpgradePageService } from './_services/buildandupgrade-page.service';
import { MarketManagementPageService } from './_services/marketmanagement-page.service';

//TableComponent
import { TableCommercialManagementPageComponent } from './table-commercialmanagement-page/table-commercialmanagement-page.component';
import { TableBuildAndUpgradePageComponent } from './table-buildandupgrade-page/table-buildandupgrade-page.component';
import { TableMarketManagementPageComponent } from './table-marketmanagement-page/table-marketmanagement-page.component';

//Edit Component
import { EditCommercialManagementModalComponent } from './table-commercialmanagement-page/components/edit-commercialmanagement-modal/edit-commercialmanagement-modal.component';
import { EditCommercialManagementMarketModalComponent } from './table-commercialmanagement-page/components/edit-market-modal/edit-market-modal.component';
import { EditCommercialManagementShoppingMallModalComponent } from './table-commercialmanagement-page/components/edit-shoppingmall-modal/edit-shoppingmall-modal.component';
import { EditCommercialManagementSuperMarketModalComponent } from './table-commercialmanagement-page/components/edit-supermarket-modal/edit-supermarket-modal.component';

import { EditBuildAndUpgradeModalComponent } from './table-buildandupgrade-page/components/edit-buildandupgrade-modal/edit-buildandupgrade-modal.component';

import { EditMarketManagementModalComponent } from './table-marketmanagement-page/components/edit-marketmanagement-modal/edit-marketmanagement-modal.component';
import { EditCommercialManagementConvenienceStoreModalComponent } from './table-commercialmanagement-page/components/edit-conveniencestore-modal/edit-conveniencestore-modal.component';
import { EditCommercialManagementSpecializedStoreModalComponent } from './table-commercialmanagement-page/components/edit-specializedstore-modal/edit-specializedstore-modal.component';
import { EditCommercialManagementGroceryModalComponent } from './table-commercialmanagement-page/components/edit-grocery-modal/edit-grocery-modal.component';
import { EditCommercialManagementLogisticsModalComponent } from './table-commercialmanagement-page/components/edit-logistics-modal/edit-logistics-modal.component';
import { AddBusinessLineModalComponent } from './table-marketmanagement-page/components/edit-businessline-modal/edit-businessline-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { MonthYearPickerCustomModule } from 'src/app/_metronic/shared/components/month-year-picker/month-year-picker-custom.module';

@NgModule({
  declarations: [
    CommercialManagementPageComponent,
    TableCommercialManagementPageComponent,
    TableBuildAndUpgradePageComponent,
    TableMarketManagementPageComponent,
    EditCommercialManagementModalComponent,
    EditCommercialManagementMarketModalComponent,
    EditCommercialManagementShoppingMallModalComponent,
    EditCommercialManagementSuperMarketModalComponent,
    EditBuildAndUpgradeModalComponent,
    EditMarketManagementModalComponent,
    EditCommercialManagementConvenienceStoreModalComponent,
    EditCommercialManagementSpecializedStoreModalComponent,
    EditCommercialManagementGroceryModalComponent,
    EditCommercialManagementLogisticsModalComponent,
    AddBusinessLineModalComponent,
  ],
  providers: [
    CommercialManagementPageService,
    BuildAndUpgradePageService,
    MarketManagementPageService,
	],
  imports: [
    CommonModule,
    CommercialManagementPageRoutingModule,
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
    MatCheckboxModule,
    MatIconModule,
    MatButtonModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    MonthYearPickerCustomModule
  ]
})
export class CommercialManagementPageModule {}
