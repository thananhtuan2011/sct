import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTarget7PageComponent } from './table-target7-page/table-page.component';
import { Target7PageComponent } from './target7-page.component';

const routes: Routes = [
  {
    path: '',
    component: Target7PageComponent,
    children: [
      {
        path: 'list',
        component: TableTarget7PageComponent,
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

export class Target7PageRoutingModule {}
