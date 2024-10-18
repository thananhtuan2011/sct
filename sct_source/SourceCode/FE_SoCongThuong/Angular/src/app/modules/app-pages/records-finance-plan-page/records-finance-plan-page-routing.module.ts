import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecordsFinancePlanPageComponent } from './records-finance-plan-page.component';
import { TableRecordsFinancePlanPageComponent } from './table-records-finance-plan-page/table-records-finance-plan-page.component';
const routes: Routes = [
  {
    path: '',
    component: RecordsFinancePlanPageComponent,
    children: [
      {
        path: 'list',
        component: TableRecordsFinancePlanPageComponent,
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
export class RecordsFinancePlanPageRoutingModule {}
