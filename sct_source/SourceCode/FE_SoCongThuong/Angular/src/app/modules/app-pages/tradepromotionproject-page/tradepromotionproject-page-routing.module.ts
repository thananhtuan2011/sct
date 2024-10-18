import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTradePromotionProjectPageComponent } from './table-tradepromotionproject-page/table-tradepromotionproject-page.component';
import { TradePromotionProjectPageComponent } from './tradepromotionproject-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradePromotionProjectPageComponent,
    children: [
      {
        path: 'list',
        component: TableTradePromotionProjectPageComponent,
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
export class TradePromotionProjectPageRoutingModule {}
