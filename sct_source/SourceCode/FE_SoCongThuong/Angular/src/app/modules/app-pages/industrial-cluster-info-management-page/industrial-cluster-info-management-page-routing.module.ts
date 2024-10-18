import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndustrialClusterInfoManagementPageComponent } from './industrial-cluster-info-management-page.component';

import { TableCateInvestmentProjectPageComponent } from './table-cate-investment-project-page/table-page.component';
import { TableCateIndustrialClusterPageComponent } from './table-cate-industrial-cluster-page/table-page.component';


const routes: Routes = [
  {
    path: '',
    component: IndustrialClusterInfoManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableCateInvestmentProjectPageComponent,
      },
      {
        path: 'CateIndustrialCluster',
        component: TableCateIndustrialClusterPageComponent,
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
export class IndustrialClusterInfoManagementPageRoutingModule {}
