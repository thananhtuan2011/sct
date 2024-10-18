import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStagePageComponent } from './table-stage-page/table-page.component';
import { StagePageComponent } from './stage-page.component';

const routes: Routes = [
  {
    path: '',
    component: StagePageComponent,
    children: [
      {
        path: 'list',
        component: TableStagePageComponent,
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
export class StagePageRoutingModule {}
