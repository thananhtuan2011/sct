import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GasPageComponent } from './gas-page.component';
import { TableGasPageComponent } from './table-gas-page/table-gas-page.component';
const routes: Routes = [
  {
    path: '',
    component: GasPageComponent,
    children: [
      {
        path: 'list',
        component: TableGasPageComponent,
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
export class GasPageRoutingModule {}
