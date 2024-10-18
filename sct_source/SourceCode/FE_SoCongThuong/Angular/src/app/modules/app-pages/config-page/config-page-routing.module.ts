import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableGroupConfigPageComponent } from './table-config-page/table-page.component';
import { GroupConfigPageComponent } from './config-page.component';

const routes: Routes = [
  {
    path: '',
    component: GroupConfigPageComponent,
    children: [
      {
        path: 'list',
        component: TableGroupConfigPageComponent,
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
export class GroupConfigPageRoutingModule {}
