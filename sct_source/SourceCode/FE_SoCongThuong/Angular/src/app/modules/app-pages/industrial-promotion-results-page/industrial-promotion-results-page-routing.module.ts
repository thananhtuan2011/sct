import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableIndustrialPromotionResultsPageComponent } from './table-industrial-promotion-results-page/table-page.component';
import { IndustrialPromotionResultsPageComponent } from './industrial-promotion-results-page.component';

const routes: Routes = [
  {
    path: '',
    component: IndustrialPromotionResultsPageComponent,
    children: [
      {
        path: 'list',
        component: TableIndustrialPromotionResultsPageComponent,
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
export class IndustrialPromotionResultsPageRoutingModule {}
