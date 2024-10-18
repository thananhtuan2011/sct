import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableRooftopSolarProjectManagementPageComponent } from './table-rooftop-solar-project-management-page/table-page.component';
import { RooftopSolarProjectManagementPageComponent } from './rooftop-solar-project-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: RooftopSolarProjectManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableRooftopSolarProjectManagementPageComponent,
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
export class RooftopSolarProjectManagementPageRoutingModule {}
