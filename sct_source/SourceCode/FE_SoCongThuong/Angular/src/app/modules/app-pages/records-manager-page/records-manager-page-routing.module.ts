import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RecordsManagerPageComponent } from './records-manager-page.component';
import { TableRecordsManagerPageComponent } from './table-records-manager-page/table-records-manager-page.component';
const routes: Routes = [
  {
    path: '',
    component: RecordsManagerPageComponent,
    children: [
      {
        path: 'list',
        component: TableRecordsManagerPageComponent,
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
export class RecordsManagerPageRoutingModule {}
