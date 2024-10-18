import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportPromotionCommerceComponent } from './RP-promotion-commerce-page.component';
import { TableReportPromotionCommerceComponent } from './RP-promotion-commerce-page/table-RP-promotion-commerce-page.component';

const routes: Routes = [
  {
    path: '',
    component: ReportPromotionCommerceComponent,
    children: [
      {
        path: 'list',
        component: TableReportPromotionCommerceComponent,
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
export class ReportPromotionCommerceModule {}
