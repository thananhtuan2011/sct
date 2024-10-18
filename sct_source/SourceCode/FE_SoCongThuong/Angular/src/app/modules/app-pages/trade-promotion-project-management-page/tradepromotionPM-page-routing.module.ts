import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTradePromotionProjectManagementPageComponent } from './table-tradepromotionPM-page/table-tradepromotionPM-page.component';
import { TradePromotionProjectManagementPageComponent } from './tradepromotionPM-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradePromotionProjectManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableTradePromotionProjectManagementPageComponent,
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
export class TradePromotionProjectManagementPageRoutingModule {}
