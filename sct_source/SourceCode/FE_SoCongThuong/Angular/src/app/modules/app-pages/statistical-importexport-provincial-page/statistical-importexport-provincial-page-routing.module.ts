import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalImportExportProvincialPageComponent } from './table-statistical-importexport-provincial-page/table-page.component';
import { StatisticalImportExportProvincialPageComponent } from './statistical-importexport-provincial-page.component';

const routes: Routes = [
  {
    path: '',
    component: StatisticalImportExportProvincialPageComponent,
    children: [
      {
        path: 'list',
        component: TableStatisticalImportExportProvincialPageComponent,
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
export class StatisticalImportExportProvincialPageRoutingModule {}
