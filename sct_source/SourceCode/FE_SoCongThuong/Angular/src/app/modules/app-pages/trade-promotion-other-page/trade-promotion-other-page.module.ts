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
import { MatIconModule } from '@angular/material/icon';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { TableTradePromotionOtherComponent } from './trade-promotion-other-page/table-trade-promotion-other-page.component';
import { EditTradePromotionOtherModalComponent } from './trade-promotion-other-page/components/edit-trade-promotion-other-modal/edit-trade-promotion-other-modal.component';
import { TradePromotionOtherModule } from './trade-promotion-other-page-routing.module';
import { TradePromotionOtherPageService } from './_services/trade-promotion-other-page.service';
import { TradePromotionOtherComponent } from './trade-promotion-other-page.component';
import { DndDirective } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive';
import { progressModule } from 'src/app/_metronic/shared/components/progress-upload/progress.component.module';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
// import { InfoTradePromotionOtherModalComponent } from './trade-promotion-other-page/components/info-trade-promotion-other-model/info-trade-promotion-other-modal.component';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    TableTradePromotionOtherComponent,
    EditTradePromotionOtherModalComponent,
    TradePromotionOtherComponent,
    //InfoTradePromotionOtherModalComponent
  ],
  providers: [
    TradePromotionOtherPageService,
	],
  imports: [
    CommonModule,
    progressModule,
    DndDirectiveModule,
    TradePromotionOtherModule,
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
    ImportDirectModule,
    DatePickerCustomModule
  ]
})
export class TradePromotionOthersModule {}