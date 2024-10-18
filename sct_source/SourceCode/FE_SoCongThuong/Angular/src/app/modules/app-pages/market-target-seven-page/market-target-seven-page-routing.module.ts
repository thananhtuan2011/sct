import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMarketTargetSevenPageComponent } from './table-market-target-seven-page/table-page.component';
import { MarketTargetSevenPageComponent } from './market-target-seven-page.component';

const routes: Routes = [
  {
    path: '',
    component: MarketTargetSevenPageComponent,
    children: [
      {
        path: 'list',
        component: TableMarketTargetSevenPageComponent,
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
export class MarketTargetSevenPageRoutingModule {}
