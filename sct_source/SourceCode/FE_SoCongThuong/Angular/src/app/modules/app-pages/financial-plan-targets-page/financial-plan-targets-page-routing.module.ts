import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableFinancialPlanTargetsPageComponent } from './table-financial-plan-targets-page/table-page.component';
import { FinancialPlanTargetsPageComponent } from './financial-plan-targets-page.component';

const routes: Routes = [
  {
    path: '',
    component: FinancialPlanTargetsPageComponent,
    children: [
      {
        path: 'list',
        component: TableFinancialPlanTargetsPageComponent,
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
export class FinancialPlanTargetsPageRoutingModule {}
