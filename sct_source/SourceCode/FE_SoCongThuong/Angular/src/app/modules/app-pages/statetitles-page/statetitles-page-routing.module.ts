import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStateTitlesPageComponent } from './table-statetitles-page/table-statetitles-page.component';
import { StateTitlesPageComponent } from './statetitles-page.component';

const routes: Routes = [
  {
    path: '',
    component: StateTitlesPageComponent,
    children: [
      {
        path: 'list',
        component: TableStateTitlesPageComponent,
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
export class StateTitlesPageRoutingModule {}
