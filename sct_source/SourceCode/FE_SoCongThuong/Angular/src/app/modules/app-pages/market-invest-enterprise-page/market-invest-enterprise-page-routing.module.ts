import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMarketInvestEnterprisePageComponent } from './table-market-invest-enterprise-page/table-page.component';
import { MarketInvestEnterprisePageComponent } from './market-invest-enterprise-page.component';

const routes: Routes = [
  {
    path: '',
    component: MarketInvestEnterprisePageComponent,
    children: [
      {
        path: 'list',
        component: TableMarketInvestEnterprisePageComponent,
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
export class MarketInvestEnterprisePageRoutingModule {}
