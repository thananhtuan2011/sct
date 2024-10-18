import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableElectricalProjectManagementPageComponent } from './table-electrical-project-management-page/table-page.component';
import { ElectricalProjectManagementPageComponent } from './electrical-project-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: ElectricalProjectManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableElectricalProjectManagementPageComponent,
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

export class ElectricalProjectManagementPageRoutingModule {}
