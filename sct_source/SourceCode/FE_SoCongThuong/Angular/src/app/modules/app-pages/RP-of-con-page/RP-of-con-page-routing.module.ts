import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RPOSOfConstructionComponent } from './RP-of-con-page.component';
import { TableRPOSOfConstructionComponent } from './RP-of-con-page/table-RP-of-con-page.component';

const routes: Routes = [
  {
    path: '',
    component: RPOSOfConstructionComponent,
    children: [
      {
        path: 'list',
        component: TableRPOSOfConstructionComponent,
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
export class RPOSOfConstructionModule {}
