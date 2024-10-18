import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GasBusinessPageComponent } from './gas-business-page.component';
import { TableGasBusinessPageComponent } from './table-gas-business-page/table-gas-business-page.component';
const routes: Routes = [
  {
    path: '',
    component: GasBusinessPageComponent,
    children: [
      {
        path: 'list',
        component: TableGasBusinessPageComponent,
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
export class GasBusinessPageRoutingModule {}
