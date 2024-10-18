import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableRegulationConformityAMPageComponent } from './table-regulation-conformity-AM-page/table-page.component';
import { RegulationConformityAMPageComponent } from './reg-conform-AM-page.component';

const routes: Routes = [
  {
    path: '',
    component: RegulationConformityAMPageComponent,
    children: [
      {
        path: 'list',
        component: TableRegulationConformityAMPageComponent,
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
export class RegulationConformityAMPageRoutingModule {}
