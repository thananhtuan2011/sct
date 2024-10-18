import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableProposedPowerProjectsPageComponent } from './table-proposed-power-projects-page/table-page.component';
import { ProposedPowerProjectsPageComponent } from './proposed-power-projects-page.component';

const routes: Routes = [
  {
    path: '',
    component: ProposedPowerProjectsPageComponent,
    children: [
      {
        path: 'list',
        component: TableProposedPowerProjectsPageComponent,
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
export class ProposedPowerProjectsPageRoutingModule {}
