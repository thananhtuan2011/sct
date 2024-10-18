import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMultiLevelSalesManagementPageComponent } from './table-multi-level-sales-management-page/table-page.component';
import { MultiLevelSalesManagementPageComponent } from './multi-level-sales-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: MultiLevelSalesManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableMultiLevelSalesManagementPageComponent,
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
export class MultiLevelSalesManagementPageRoutingModule {}
