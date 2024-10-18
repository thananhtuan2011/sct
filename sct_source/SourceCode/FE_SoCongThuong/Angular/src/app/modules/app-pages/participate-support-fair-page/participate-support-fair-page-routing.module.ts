import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ParticipateSupportFairPageComponent } from './participate-support-fair-page.component';

import { TableParticipateSupportFairPageComponent } from './table-participate-support-fair-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: ParticipateSupportFairPageComponent,
    children: [
      {
        path: 'list',
        component: TableParticipateSupportFairPageComponent,
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
export class ParticipateSupportFairPageRoutingModule {}
