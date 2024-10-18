import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableAdministrativeProcedureFieldComponent } from './table-administrative-procedure-field-page/table-page.component';
import { AdministrativeProcedureFieldComponent } from './administrative-procedure-field-page.component';

const routes: Routes = [
  {
    path: '',
    component: AdministrativeProcedureFieldComponent,
    children: [
      {
        path: 'list',
        component: TableAdministrativeProcedureFieldComponent,
      },{
        path: 'list',
        component: TableAdministrativeProcedureFieldComponent,
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
export class AdministrativeProcedureFieldRoutingModule {}
