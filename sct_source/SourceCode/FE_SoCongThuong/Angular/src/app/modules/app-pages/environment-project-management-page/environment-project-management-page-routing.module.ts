import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableEnvironmentProjectManagementPageComponent } from './table-environment-project-management-page/table-page.component';
import { EnvironmentProjectManagementPageComponent } from './environment-project-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: EnvironmentProjectManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableEnvironmentProjectManagementPageComponent,
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
export class EnvironmentProjectManagementPageRoutingModule {}
