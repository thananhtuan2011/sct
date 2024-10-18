import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TablePageComponent } from './table-management-electricity-activities-page/table-page.component';
import { ManagementElectricityActivitiesPageComponent } from './management-electricity-activities-page.component';

const routes: Routes = [
  {
    path: '',
    component: ManagementElectricityActivitiesPageComponent,
    children: [
      {
        path: 'list',
        component: TablePageComponent,
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

export class ManagementElectricityActivitiesPageRoutingModule {}
