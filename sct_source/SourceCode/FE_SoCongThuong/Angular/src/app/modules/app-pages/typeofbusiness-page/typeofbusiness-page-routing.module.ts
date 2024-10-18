import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTypeOfBusinessPageComponent } from './table-typeofbusiness-page/table-typeofbusiness-page.component';
import { TypeOfBusinessPageComponent } from './typeofbusiness-page.component';

const routes: Routes = [
  {
    path: '',
    component: TypeOfBusinessPageComponent,
    children: [
      {
        path: 'list',
        component: TableTypeOfBusinessPageComponent,
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
export class TypeOfBusinessPageRoutingModule {}
