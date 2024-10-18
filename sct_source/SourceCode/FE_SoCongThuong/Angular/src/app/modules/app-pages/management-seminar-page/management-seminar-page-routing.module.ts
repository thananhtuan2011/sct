import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ManagementSeminarPageComponent } from './management-seminar-page.component';

import { TableManagementSeminarPageComponent } from './table-management-seminar-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: ManagementSeminarPageComponent,
    children: [
      {
        path: 'list',
        component: TableManagementSeminarPageComponent,
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
export class ManagementSeminarPageRoutingModule {}
