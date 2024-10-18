import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalImportExportDistrictPageComponent } from './table-statistical-importexport-district-page/table-page.component';
import { StatisticalImportExportDistrictPageComponent } from './statistical-importexport-district-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalImportExportDistrictPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalImportExportDistrictPageComponent,
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
export class StatisticalImportExportDistrictPageRoutingModule {}
