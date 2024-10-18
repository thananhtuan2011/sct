import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RPOSOfInvestmentnComponent } from './RP-of-investment-page.component';
import { TableRPOSOfInvestmentnComponent } from './RP-of-investment-page/table-RP-of-investment-page.component';

const routes: Routes = [
  {
    path: '',
    component: RPOSOfInvestmentnComponent,
    children: [
      {
        path: 'list',
        component: TableRPOSOfInvestmentnComponent,
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
export class RPOSOfInvestmentnModule {}
