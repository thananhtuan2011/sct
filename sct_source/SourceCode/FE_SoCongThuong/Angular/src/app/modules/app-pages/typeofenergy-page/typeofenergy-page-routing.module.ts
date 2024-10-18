import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTypeOfEnergyPageComponent } from './table-typeofenergy-page/table-typeofenergy-page.component';
import { TypeOfEnergyPageComponent } from './typeofenergy-page.component';

const routes: Routes = [
  {
    path: '',
    component: TypeOfEnergyPageComponent,
    children: [
      {
        path: 'list',
        component: TableTypeOfEnergyPageComponent,
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
export class TypeOfEnergyPageRoutingModule {}
