import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTradeFairOrganizationCertificationPageComponent } from './table-trade-fair-organization-page/table-page.component';
import { TradeFairOrganizationPageComponent } from './trade-fair-organization-page.component';

const routes: Routes = [
  {
    path: '',
    component: TradeFairOrganizationPageComponent,
    children: [
      {
        path: 'list',
        component: TableTradeFairOrganizationCertificationPageComponent,
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
export class TradeFairOrganizationPageRoutingModule {}
