import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTypeOfMarketPageComponent } from './table-typeofmarket-page/table-typeofmarket-page.component';
import { TypeOfMarketPageComponent } from './typeofmarket-page.component';

const routes: Routes = [
  {
    path: '',
    component: TypeOfMarketPageComponent,
    children: [
      {
        path: 'list',
        component: TableTypeOfMarketPageComponent,
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
export class TypeOfMarketPageRoutingModule {}
