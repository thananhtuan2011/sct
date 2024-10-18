import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RPIndustrialComponent } from './RP-industrial-page.component';
import { TableRPIndustrialComponent } from './RP-industrial-page/table-RP-industrial-page.component';

const routes: Routes = [
  {
    path: '',
    component: RPIndustrialComponent,
    children: [
      {
        path: 'list',
        component: TableRPIndustrialComponent,
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
export class RPIndustrialModule {}
