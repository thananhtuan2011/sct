import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStateUnitsPageComponent } from './table-stateunits-page/table-stateunits-page.component';
import { StateUnitsPageComponent } from './stateunits-page.component';

const routes: Routes = [
  {
    path: '',
    component: StateUnitsPageComponent,
    children: [
      {
        path: 'list',
        component: TableStateUnitsPageComponent,
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
export class StateUnitsPageRoutingModule {}
