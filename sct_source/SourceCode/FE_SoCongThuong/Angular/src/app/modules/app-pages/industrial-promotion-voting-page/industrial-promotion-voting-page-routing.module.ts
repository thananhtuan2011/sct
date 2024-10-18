import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableResultsIndustrialPromotionVotingPageComponent } from './table-industrial-promotion-voting-page/table-page.component';
import { ResultsIndustrialPromotionVotingPageComponent } from './industrial-promotion-voting-page.component';

const routes: Routes = [
  {
    path: '',
    component: ResultsIndustrialPromotionVotingPageComponent,
    children: [
      {
        path: 'list',
        component: TableResultsIndustrialPromotionVotingPageComponent,
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
export class ResultsIndustrialPromotionVotingPageRoutingModule {}
