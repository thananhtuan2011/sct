import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCommunePageComponent } from './table-commune-page/table-commune-page.component';
import { CommunePageComponent } from './commune-page.component';

const routes: Routes = [
  {
    path: '',
    component: CommunePageComponent,
    children: [
      {
        path: 'list',
        component: TableCommunePageComponent,
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
export class CommunePageRoutingModule {}
