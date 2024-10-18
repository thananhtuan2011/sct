import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableReportAdministrativeProceduresPageComponent } from './table-report-administrative-procedures-page/table-page.component';
import { ReportAdministrativeProceduresPageComponent } from './report-administrative-procedures-page.component';

const routes: Routes = [
  {
    path: '',
    component: ReportAdministrativeProceduresPageComponent,
    children: [
      {
        path: 'list',
        component: TableReportAdministrativeProceduresPageComponent,
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
export class ReportAdministrativeProceduresPageRoutingModule {}
