import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableIndustryPromotionReportPageComponent } from './table-industry-promotion-report-page/table-page.component';
import { IndustryPromotionReportPageComponent } from './industry-promotion-report-page.component';

const routes: Routes = [
  {
    path: '',
    component: IndustryPromotionReportPageComponent,
    children: [
      {
        path: 'list',
        component: TableIndustryPromotionReportPageComponent,
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
export class IndustryPromotionReportPageRoutingModule {}
