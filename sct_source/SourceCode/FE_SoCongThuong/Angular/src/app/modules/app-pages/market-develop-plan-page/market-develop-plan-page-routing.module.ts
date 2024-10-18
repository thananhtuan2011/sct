import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMarketDevelopPlanPageComponent } from './table-market-develop-plan-page/table-page.component';
import { MarketDevelopPlanPageComponent } from './market-develop-plan-page.component';

const routes: Routes = [
  {
    path: '',
    component: MarketDevelopPlanPageComponent,
    children: [
      {
        path: 'list',
        component: TableMarketDevelopPlanPageComponent,
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
export class MarketDevelopPlanPageRoutingModule {}
