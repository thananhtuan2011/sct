import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableReportIndexIndustryPageComponent } from './table-report-index-industry-page/table-page.component';
import { ReportIndexIndustryPageComponent } from './report-index-industry-page.component';

const routes: Routes = [
  {
    path: '',
    component: ReportIndexIndustryPageComponent,
    children: [
      {
        path: 'list',
        component: TableReportIndexIndustryPageComponent,
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
export class ReportIndexIndustryPageRoutingModule {}
