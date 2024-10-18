import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TradePromotionActivityReportPageComponent } from './trade-promotion-activity-report-page.component';

import { TableTradePromotionActivityReportPageComponent } from './table-trade-promotion-activity-report-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradePromotionActivityReportPageComponent,
    children: [
      {
        path: 'list',
        component: TableTradePromotionActivityReportPageComponent,
      },
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: '**', redirectTo: 'list', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class TradePromotionActivityReportPageRoutingModule {}
