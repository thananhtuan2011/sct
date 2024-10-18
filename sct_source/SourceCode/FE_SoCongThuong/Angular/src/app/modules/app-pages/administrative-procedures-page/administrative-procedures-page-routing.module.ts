import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableAdministrativeProceduresPageComponent } from './table-administrative-procedures-page/table-page.component';
import { AdministrativeProceduresPageComponent } from './administrative-procedures-page.component';

const routes: Routes = [
  {
    path: '',
    component: AdministrativeProceduresPageComponent,
    children: [
      {
        path: 'list',
        component: TableAdministrativeProceduresPageComponent,
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
export class AdministrativeProceduresPageRoutingModule {}
