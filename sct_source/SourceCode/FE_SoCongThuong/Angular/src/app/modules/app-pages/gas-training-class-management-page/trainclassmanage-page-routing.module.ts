import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTrainingClassManagementPageComponent } from './table-trainingclassmanagement-page/table-page.component';
import { TrainingClassManagementPageComponent } from './trainclassmanage-page.component';

const routes: Routes = [
  {
    path: '',
    component: TrainingClassManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableTrainingClassManagementPageComponent,
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
export class TrainingClassManagementPageRoutingModule {}
