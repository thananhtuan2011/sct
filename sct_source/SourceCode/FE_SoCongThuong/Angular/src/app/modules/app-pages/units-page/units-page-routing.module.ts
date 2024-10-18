import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UnitsPageComponent } from './units-page.component';
import { TableUnitPageComponent } from './table-units-page/table-units-page.component';
const routes: Routes = [
  {
    path: '',
    component: UnitsPageComponent,
    children: [
      {
        path: 'list',
        component: TableUnitPageComponent,
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
export class UnitsPageRoutingModule {}
