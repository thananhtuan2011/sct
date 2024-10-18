import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableElectricityInspectorCardPageComponent } from './table-electricity-inspector-card-page/table-page.component';
import { ElectricityInspectorCardPageComponent } from './electricity-inspector-card-page.component';

const routes: Routes = [
  {
    path: '',
    component: ElectricityInspectorCardPageComponent,
    children: [
      {
        path: 'list',
        component: TableElectricityInspectorCardPageComponent,
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
export class ElectricityInspectorCardPageRoutingModule {}
