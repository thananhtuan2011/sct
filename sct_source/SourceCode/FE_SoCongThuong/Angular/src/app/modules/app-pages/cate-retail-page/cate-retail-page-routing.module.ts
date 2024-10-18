import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CateRetailComponent } from './cate-retail-page.component';
import { TableCateRetailModalComponent } from './cate-retail-page/table-cate-retail-page.component';

const routes: Routes = [
  {
    path: '',
    component: CateRetailComponent,
    children: [
      {
        path: 'list',
        component: TableCateRetailModalComponent,
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
export class CateRetailRoutingModule {}
