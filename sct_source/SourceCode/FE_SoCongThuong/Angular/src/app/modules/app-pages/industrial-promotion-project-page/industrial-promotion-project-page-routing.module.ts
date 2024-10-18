import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { IndustrialPromotionProjectPageComponent } from './industrial-promotion-project-page.component';

import { TableIndustrialPromotionProjectPageComponent } from './table-industrial-promotion-project-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: IndustrialPromotionProjectPageComponent,
    children: [
      {
        path: 'list',
        component: TableIndustrialPromotionProjectPageComponent,
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
export class IndustrialPromotionProjectPageRoutingModule {}
