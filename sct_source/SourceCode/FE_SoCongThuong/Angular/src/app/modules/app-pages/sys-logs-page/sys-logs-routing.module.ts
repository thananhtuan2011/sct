import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SysLogsPageComponent } from './sys-logs-page.component';
import { TableSysLogsPageComponent } from './table-sys-logs-page/table-sys-logs-page.component';

const routes: Routes = [
  {
    path: '',
    component: SysLogsPageComponent,
    children: [
      {
        path: 'list',
        component: TableSysLogsPageComponent,
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
export class SysLogsPageRoutingModule {}
