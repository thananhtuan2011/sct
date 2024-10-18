import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { IndustrialManagementTargetPageComponent } from './target-page.component';

import { TableIndustrialManagementTargetPageComponent } from './table-indus-manage-target-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: IndustrialManagementTargetPageComponent,
    children: [
      {
        path: 'list',
        component: TableIndustrialManagementTargetPageComponent,
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
export class IndustrialManagementTargetPageRoutingModule {}
