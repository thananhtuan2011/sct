import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CateProjectComponent } from './cate-project-page.component';
import { TableCateProjectModalComponent } from './cate-project-page/table-cate-project-page.component';

const routes: Routes = [
  {
    path: '',
    component: CateProjectComponent,
    children: [
      {
        path: 'list',
        component: TableCateProjectModalComponent,
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
export class CateProjectRoutingModule {}
