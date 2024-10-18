import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMarketPlanInformationPageComponent } from './table-market-plan-information-page/table-page.component';
import { MarketPlanInformationPageComponent } from './market-plan-information-page.component';

const routes: Routes = [
  {
    path: '',
    component: MarketPlanInformationPageComponent,
    children: [
      {
        path: 'list',
        component: TableMarketPlanInformationPageComponent,
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
export class MarketPlanInformationPageRoutingModule {}
