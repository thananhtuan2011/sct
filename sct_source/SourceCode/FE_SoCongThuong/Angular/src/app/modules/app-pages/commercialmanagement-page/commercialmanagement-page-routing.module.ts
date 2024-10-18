import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCommercialManagementPageComponent } from './table-commercialmanagement-page/table-commercialmanagement-page.component';
import { TableBuildAndUpgradePageComponent } from './table-buildandupgrade-page/table-buildandupgrade-page.component';
import { TableMarketManagementPageComponent } from './table-marketmanagement-page/table-marketmanagement-page.component';
import { CommercialManagementPageComponent } from './commercialmanagement-page.component';

const routes: Routes = [
  {
    path: '',
    component: CommercialManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableCommercialManagementPageComponent,
      },
      {
        path: 'buildandupgrade',
        component: TableBuildAndUpgradePageComponent,
      },
            {
        path: 'marketmanagement',
        component: TableMarketManagementPageComponent,
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
export class CommercialManagementPageRoutingModule {}
