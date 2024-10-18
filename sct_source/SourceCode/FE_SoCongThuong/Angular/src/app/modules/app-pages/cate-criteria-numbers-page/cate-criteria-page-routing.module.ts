import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CateCriteriaComponent } from './cate-criteria-page.component';
import { TableCriteriaModalComponent } from './cate-criteria-page/table-cate-criteria-page.component';

const routes: Routes = [
  {
    path: '',
    component: CateCriteriaComponent,
    children: [
      {
        path: 'list',
        component: TableCriteriaModalComponent,
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
export class CateCriteriaRoutingModule {}
