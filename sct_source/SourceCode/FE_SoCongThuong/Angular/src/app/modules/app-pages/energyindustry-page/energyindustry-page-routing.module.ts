import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableEnergyIndustryPageComponent } from './table-energyindustry-page/table-energyindustry-page.component';
import { EnergyIndustryPageComponent } from './energyindustry-page.component';

const routes: Routes = [
  {
    path: '',
    component: EnergyIndustryPageComponent,
    children: [
      {
        path: 'list',
        component: TableEnergyIndustryPageComponent,
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
export class EnergyIndustryPageRoutingModule {}
