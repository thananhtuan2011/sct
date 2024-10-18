import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlcoholBusinessComponent } from './alcohol-bus-page.component';
import { TableAlcoholBusinessModalComponent } from './alcohol-bus-page/table-alcohol-bus-page.component';

const routes: Routes = [
  {
    path: '',
    component: AlcoholBusinessComponent,
    children: [
      {
        path: 'list',
        component: TableAlcoholBusinessModalComponent,
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
export class AlcoholBusinessRoutingModule {}
