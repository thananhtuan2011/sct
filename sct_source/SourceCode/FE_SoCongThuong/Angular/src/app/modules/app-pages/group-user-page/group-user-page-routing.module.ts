import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GroupUserPageComponent } from './group-user-page.component';
import { TableGroupUserComponent } from './table-group-user/table-group-user.component';
const routes: Routes = [
  {
    path: '',
    component: GroupUserPageComponent,
    children: [
      {
        path: 'list',
        component: TableGroupUserComponent,
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
export class GroupUserPageRoutingModule {}
