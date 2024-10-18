import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableDistrictTradeStatisticsPageComponent } from './table-district-trade-statistics-page/table-page.component';
import { DistrictTradeStatisticsPageComponent } from './district-trade-statistics-page.component';

const routes: Routes = [
  {
    path: '',
    component: DistrictTradeStatisticsPageComponent,
    children: [
      {
        path: 'list',
        component: TableDistrictTradeStatisticsPageComponent,
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
export class DistrictTradeStatisticsPageRoutingModule {}
