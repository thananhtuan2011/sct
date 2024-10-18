import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableChemicalBusinessManagementPageComponent } from './table-chemical-business-management-page/table-page.component';
import { ChemicalBusinessManagementPageComponent } from './chemical-business-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: ChemicalBusinessManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableChemicalBusinessManagementPageComponent,
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
export class ChemicalBusinessManagementPageRoutingModule {}
