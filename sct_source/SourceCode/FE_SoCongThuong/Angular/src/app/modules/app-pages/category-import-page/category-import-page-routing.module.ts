import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoryImportPageComponent } from './category-import-page.component';
import { TableCategoryImportPageComponent } from './table-category-import-page/table-category-import-page.component';
const routes: Routes = [
  {
    path: '',
    component: CategoryImportPageComponent,
    children: [
      {
        path: 'list',
        component: TableCategoryImportPageComponent,
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
export class CategoryImportPageRoutingModule {}
