import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableManagementFirePreventionPageComponent } from './table-management-fire-prevention-page/table-page.component';
import { ManagementFirePreventionPageComponent } from './management-fire-prevention-page.component';

const routes: Routes = [
  {
    path: '',
    component: ManagementFirePreventionPageComponent,
    children: [
      {
        path: 'list',
        component: TableManagementFirePreventionPageComponent,
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
export class ManagementFirePreventionPageRoutingModule {}
