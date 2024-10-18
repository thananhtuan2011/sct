import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalAlcoholBusinessPageComponent } from './table-statistical-alcoholbusiness-page/table-page.component';
import { StatisticalAlcoholBusinessPageComponent } from './statistical-alcoholbusiness-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalAlcoholBusinessPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalAlcoholBusinessPageComponent,
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
export class StatisticalAlcoholBusinessPageRoutingModule {}
