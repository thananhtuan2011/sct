import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableProcessAdministrativeProceduresPageComponent } from './table-process-administrative-procedures-page/table-page.component';
import { ProcessAdministrativeProceduresPageComponent } from './process-administrative-procedures-page.component';

const routes: Routes = [
  {
    path: '',
    component: ProcessAdministrativeProceduresPageComponent,
    children: [
      {
        path: 'list',
        component: TableProcessAdministrativeProceduresPageComponent,
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
export class ProcessAdministrativeProceduresPageRoutingModule {}
