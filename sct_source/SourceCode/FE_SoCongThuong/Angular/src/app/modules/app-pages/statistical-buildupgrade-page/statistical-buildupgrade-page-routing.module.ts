import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalBuildUpgradePageComponent } from './table-statistical-buildupgrade-page/table-page.component';
import { StatisticalBuildUpgradePageComponent } from './statistical-buildupgrade-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalBuildUpgradePageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalBuildUpgradePageComponent,
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
export class StatisticalBuildUpgradePageRoutingModule {}
