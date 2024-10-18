import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableNewRuralCriteriaPageComponent } from './table-new-rural-criteria-page/table-page.component';
import { NewRuralCriteriaPageComponent } from './new-rural-criteria-page.component';

const routes: Routes = [
  {
    path: '',
    component: NewRuralCriteriaPageComponent,
    children: [
      {
        path: 'list',
        component: TableNewRuralCriteriaPageComponent,
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

export class NewRuralCriteriaPageRoutingModule {}
