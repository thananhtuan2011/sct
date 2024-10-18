import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableProvincialTradeStatisticsPageComponent } from './table-provincial-trade-statistics-page/table-page.component';
import { ProvincialTradeStatisticsPageComponent } from './provincial-trade-statistics-page.component';

const routes: Routes = [
  {
    path: '',
    component: ProvincialTradeStatisticsPageComponent,
    children: [
      {
        path: 'list',
        component: TableProvincialTradeStatisticsPageComponent,
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
export class ProvincialTradeStatisticsPageRoutingModule {}
