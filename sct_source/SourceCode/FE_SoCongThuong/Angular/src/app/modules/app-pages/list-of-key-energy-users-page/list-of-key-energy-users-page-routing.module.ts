import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableListOfKeyEnergyUsersPageComponent } from './table-list-of-key-energy-users-page/table-page.component';
import { ListOfKeyEnergyUsersPageComponent } from './list-of-key-energy-users-page.component';

const routes: Routes = [
  {
    path: '',
    component: ListOfKeyEnergyUsersPageComponent,
    children: [
      {
        path: 'list',
        component: TableListOfKeyEnergyUsersPageComponent,
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

export class ListOfKeyEnergyUsersPageRoutingModule {}
