import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TradePromotionOtherComponent } from './trade-promotion-other-page.component';
import { TableTradePromotionOtherComponent } from './trade-promotion-other-page/table-trade-promotion-other-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradePromotionOtherComponent,
    children: [
      {
        path: 'list',
        component: TableTradePromotionOtherComponent,
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
export class TradePromotionOtherModule {}
