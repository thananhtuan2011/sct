import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomPageComponent } from './custom-page.component';
import { TableCustomPageComponent } from './table-custom-page/table-custom-page.component';
const routes: Routes = [
  {
    path: '',
    component: CustomPageComponent,
    children: [
      {
        path: 'list',
        component: TableCustomPageComponent,
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
export class CustomPageRoutingModule {}
