import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableParticipateTradePromotionsComponent } from './participate-trade-promotion-page/table-participate-trade-promotion-page.component';
import { ParticipateTradePromotionComponent } from './participate-trade-promotion.component';

const routes: Routes = [
  {
    path: '',
    component: ParticipateTradePromotionComponent,
    children: [
      {
        path: 'list',
        component: TableParticipateTradePromotionsComponent,
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
export class InfoParticipateTradePromotionRoutingModule {}
