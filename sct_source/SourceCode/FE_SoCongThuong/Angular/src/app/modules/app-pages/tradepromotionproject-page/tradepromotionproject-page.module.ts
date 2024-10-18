import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TradePromotionProjectPageRoutingModule } from './tradepromotionproject-page-routing.module';
import { TableTradePromotionProjectPageComponent } from './table-tradepromotionproject-page/table-tradepromotionproject-page.component';
import { TradePromotionProjectPageComponent } from './tradepromotionproject-page.component';
import { TradePromotionProjectPageService } from './_services/tradepromotionproject-page.service';
import { EditTradePromotionProjectModalComponent } from './table-tradepromotionproject-page/components/edit-tradepromotionproject-modal/edit-tradepromotionproject-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TradePromotionProjectPageComponent,
    TableTradePromotionProjectPageComponent,
    EditTradePromotionProjectModalComponent
  ],
  providers: [
    TradePromotionProjectPageService
	],
  imports: [
    CommonModule,
    TradePromotionProjectPageRoutingModule,
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
    ImportDirectModule
  ]
})
export class TradePromotionProjectPageModule {}