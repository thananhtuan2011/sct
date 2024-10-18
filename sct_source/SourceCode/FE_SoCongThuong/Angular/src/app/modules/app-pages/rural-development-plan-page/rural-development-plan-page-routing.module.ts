import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableRuralDevelopmentPlanPageComponent } from './table-rural-development-plan-page/table-page.component';
import { RuralDevelopmentPlanPageComponent } from './rural-development-plan-page.component';

const routes: Routes = [
  {
    path: '',
    component: RuralDevelopmentPlanPageComponent,
    children: [
      {
        path: 'list',
        component: TableRuralDevelopmentPlanPageComponent,
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
export class RuralDevelopmentPlanPageRoutingModule {}
