import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalProductAlcolPageComponent } from './table-statistical-product-alcol-page/table-page.component';
import { StatisticalProductAlcolPageComponent } from './statistical-product-alcol-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalProductAlcolPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalProductAlcolPageComponent,
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
export class StatisticalProductAlcolPageRoutingModule {}
