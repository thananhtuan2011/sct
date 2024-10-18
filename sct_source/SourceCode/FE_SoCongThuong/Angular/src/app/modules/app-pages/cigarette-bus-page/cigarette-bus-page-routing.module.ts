import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CigaretteBusinessComponent } from './cigarette-bus-page.component';
import { TableCigeratteBusinessModalComponent } from './cigarette-bus-page/table-cigarette-bus-page.component';

const routes: Routes = [
  {
    path: '',
    component: CigaretteBusinessComponent,
    children: [
      {
        path: 'list',
        component: TableCigeratteBusinessModalComponent,
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
export class CigaretteBusinessRoutingModule {}
