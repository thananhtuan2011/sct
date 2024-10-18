import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableBusinessLinePageComponent } from './table-businessline-page/table-page.component';
import { BusinessLinePageComponent } from './business-line-page.component';

const routes: Routes = [
  {
    path: '',
    component: BusinessLinePageComponent,
    children: [
      {
        path: 'list',
        component: TableBusinessLinePageComponent,
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
export class BusinessLinePageRoutingModule {}
