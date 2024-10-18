import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableImportGoodsPageComponent } from './table-importgoods-page/table-importgoods-page.component';
import { TableExportGoodsPageComponent } from './table-exportgoods-page/table-exportgoods-page.component';
import { TableImportExportInfoPageComponent } from './table-importexportinfo-page/table-importexportinfo-page.component';
import { ImportExportPageComponent } from './importexport-page.component';

const routes: Routes = [
  {
    path: '',
    component: ImportExportPageComponent,
    children: [
      {
        path: 'importgoods',
        component: TableImportGoodsPageComponent,
      },
      {
        path: 'exportgoods',
        component: TableExportGoodsPageComponent,
      },
      {
        path: 'info',
        component: TableImportExportInfoPageComponent,
      },
      { path: '', redirectTo: 'importgoods', pathMatch: 'full' },
      { path: '**', redirectTo: 'importgoods', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ImportExportPageRoutingModule {}
