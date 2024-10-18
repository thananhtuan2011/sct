import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableAdminFormalitiesPageComponent } from './table-adminformalities-page/table-adminformalities-page.component';
import { AdminFormalitiesPageComponent } from './adminformalities-page.component';

const routes: Routes = [
  {
    path: '',
    component: AdminFormalitiesPageComponent,
    children: [
      {
        path: 'list',
        component: TableAdminFormalitiesPageComponent,
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
export class AdminFormalitiesPageRoutingModule {}
