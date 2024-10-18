import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableApprovedPowerProjectsPageComponent } from './table-approved-power-projects-page/table-page.component';
import { ApprovedPowerProjectsPageComponent } from './approved-power-projects-page.component';

const routes: Routes = [
  {
    path: '',
    component: ApprovedPowerProjectsPageComponent,
    children: [
      {
        path: 'list',
        component: TableApprovedPowerProjectsPageComponent,
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
export class ApprovedPowerProjectsPageRoutingModule {}
