import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableProductOcopPageComponent } from './table-product-ocop-page/table-page.component';
import { ProductOcopPageComponent } from './product-ocop-page.component';

const routes: Routes = [
  {
    path: '',
    component: ProductOcopPageComponent,
    children: [
      {
        path: 'list',
        component: TableProductOcopPageComponent,
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
export class ProductOcopPageRoutingModule {}
