import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalPetroleumPageComponent } from './table-statistical-petroleum-page/table-page.component';
import { StatisticalPetroleumPageComponent } from './statistical-petroleum-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalPetroleumPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalPetroleumPageComponent,
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
export class StatisticalPetroleumPageRoutingModule {}
