import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalFairParticipantPageComponent } from './table-statistical-fair-participant-page/table-page.component';
import { StatisticalFairParticipantPageComponent } from './statistical-fair-participant-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalFairParticipantPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalFairParticipantPageComponent,
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
export class StatisticalFairParticipantPageRoutingModule {}
