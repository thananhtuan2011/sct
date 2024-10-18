import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTotalRetailSalePageComponent } from './table-total-retail-sale-page/table-page.component';
import { TotalRetailSalePageComponent } from './total-retail-sale-page.component';

const routes: Routes = [
  {
    path: '',
    component: TotalRetailSalePageComponent,
    children: [
      {
        path: 'list',
        component: TableTotalRetailSalePageComponent,
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
export class TotalRetailSalePageRoutingModule {}
