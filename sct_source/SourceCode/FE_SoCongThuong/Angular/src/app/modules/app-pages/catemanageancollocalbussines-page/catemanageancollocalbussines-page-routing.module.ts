import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCateManageAncolLocalBussinesPageComponent } from './table-catemanageancollocalbussines-page/table-page.component';
import { CateManageAncolLocalBussinesPageComponent } from './catemanageancollocalbussines-page.component';

const routes: Routes = [
  {
    path: '',
    component: CateManageAncolLocalBussinesPageComponent,
    children: [
      {
        path: 'list',
        component: TableCateManageAncolLocalBussinesPageComponent,
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
export class CateManageAncolLocalBussinesPageRoutingModule {}
