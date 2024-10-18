import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTarget1708PageComponent } from './table-target1708-page/table-page.component';
import { Target1708PageComponent } from './target1708-page.component';

const routes: Routes = [
  {
    path: '',
    component: Target1708PageComponent,
    children: [
      {
        path: 'list',
        component: TableTarget1708PageComponent,
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

export class Target1708PageRoutingModule {}
