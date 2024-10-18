import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableFoodSafetyCertificatePageComponent } from './table-food-safety-certificate-page/table-page.component';
import { FoodSafetyCertificatePageComponent } from './food-safety-certificate-page.component';

const routes: Routes = [
  {
    path: '',
    component: FoodSafetyCertificatePageComponent,
    children: [
      {
        path: 'list',
        component: TableFoodSafetyCertificatePageComponent,
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

export class FoodSafetyCertificatePageRoutingModule {}
