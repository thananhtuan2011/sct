import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DistrictPageComponent } from './district-page.component';
import { TableDistrictPageComponent } from './table-district-page/table-district-page.component';
const routes: Routes = [
  {
    path: '',
    component: DistrictPageComponent,
    children: [
      {
        path: 'list',
        component: TableDistrictPageComponent,
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
export class DistrictPageRoutingModule {}
