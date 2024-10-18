import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalMarketPageComponent } from './table-statistical-market-page/table-page.component';
import { StatisticalMarketPageComponent } from './statistical-market-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalMarketPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalMarketPageComponent,
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
export class StatisticalMarketPageRoutingModule {}
