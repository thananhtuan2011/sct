import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableChemicalSafetyCertificatePageComponent } from './table-chemical-safety-certificate-page/table-page.component';
import { ChemicalSafetyCertificatePageComponent } from './chemical-safety-certificate-page.component';

const routes: Routes = [
  {
    path: '',
    component: ChemicalSafetyCertificatePageComponent,
    children: [
      {
        path: 'list',
        component: TableChemicalSafetyCertificatePageComponent,
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

export class ChemicalSafetyCertificatePageRoutingModule {}
