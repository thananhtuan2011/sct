import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTrainingManagementPageComponent } from './table-training-management-page/table-page.component';
import { TrainingManagementPageComponent } from './training-management-page.component';

const routes: Routes = [
  {
    path: '',
    component: TrainingManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableTrainingManagementPageComponent,
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
export class TrainingManagementPageRoutingModule {}
