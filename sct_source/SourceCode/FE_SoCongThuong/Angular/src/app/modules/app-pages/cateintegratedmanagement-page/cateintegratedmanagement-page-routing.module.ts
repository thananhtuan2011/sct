import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCateIntegratedManagementPageComponent } from './table-cateintegratedmanagement-page/table-page.component';
import { CateIntegratedManagementPageComponent } from './cateintegratedmanagement-page.component';

const routes: Routes = [
  {
    path: '',
    component: CateIntegratedManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableCateIntegratedManagementPageComponent,
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
export class CateIntegratedManagementPageRoutingModule {}
