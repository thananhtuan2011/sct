import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCommuneElectricityManagementPageComponent } from './table-electricity-management-page/table-page.component';
import { CommuneElectricityManagementPageComponent } from './electricity-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: CommuneElectricityManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableCommuneElectricityManagementPageComponent,
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

export class CommuneElectricityManagementPageRoutingModule {}
