import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ManageArchiveRecordsPageComponent } from './manage-archive-records-page.component';
import { TableManageArchiveRecordsPageComponent } from './table-manage-archive-records-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: ManageArchiveRecordsPageComponent,
    children: [
      {
        path: 'list',
        component: TableManageArchiveRecordsPageComponent,
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

export class ManageArchiveRecordsPageRoutingModule {}
