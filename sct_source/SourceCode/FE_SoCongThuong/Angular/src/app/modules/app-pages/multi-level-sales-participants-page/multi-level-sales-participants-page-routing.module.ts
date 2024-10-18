import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableMultiLevelSalesParticipantsPageComponent } from './table-multi-level-sales-participants-page/table-page.component';
import { MultiLevelSalesParticipantsPageComponent } from './multi-level-sales-participants-page.component';

const routes: Routes = [
  {
    path: '',
    component: MultiLevelSalesParticipantsPageComponent,
    children: [
      {
        path: 'list',
        component: TableMultiLevelSalesParticipantsPageComponent,
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
export class MultiLevelSalesParticipantsPageRoutingModule {}
