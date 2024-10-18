import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTypeOfProfessionPageComponent } from './table-typeofprofession-page/table-typeofprofession-page.component';
import { TypeOfProfessionPageComponent } from './typeofprofession-page.component';

const routes: Routes = [
  {
    path: '',
    component: TypeOfProfessionPageComponent,
    children: [
      {
        path: 'list',
        component: TableTypeOfProfessionPageComponent,
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
export class TypeOfProfessionPageRoutingModule {}
