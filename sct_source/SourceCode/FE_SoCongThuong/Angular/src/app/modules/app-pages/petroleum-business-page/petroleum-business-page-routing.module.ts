import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PetroleumBusinessComponent } from './petroleum-business-page.component';
import { TablePetroleumBusinessModalComponent } from './petroleum-business-page/table-petroleum-business-page.component';

const routes: Routes = [
  {
    path: '',
    component: PetroleumBusinessComponent,
    children: [
      {
        path: 'list',
        component: TablePetroleumBusinessModalComponent,
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
export class PetroleumBusinessRoutingModule {}
