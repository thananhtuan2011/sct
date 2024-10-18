import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableElectricOperatingUnitsPageComponent } from './table-electric-operating-units-page/table-page.component';
import { ElectricOperatingUnitsPageComponent } from './electric-operating-units-page.component';

const routes: Routes = [
  {
    path: '',
    component: ElectricOperatingUnitsPageComponent,
    children: [
      {
        path: 'list',
        component: TableElectricOperatingUnitsPageComponent,
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

export class ElectricOperatingUnitsPageRoutingModule {}
