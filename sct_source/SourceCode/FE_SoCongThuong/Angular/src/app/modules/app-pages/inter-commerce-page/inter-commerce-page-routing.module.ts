import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InterCommerceComponent } from './inter-commerce-page.component';
import { TableInterCommerceModalComponent } from './inter-commerce-page/table-inter-commerce-page.component';

const routes: Routes = [
  {
    path: '',
    component: InterCommerceComponent,
    children: [
      {
        path: 'list',
        component: TableInterCommerceModalComponent,
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
export class InterCommerceRoutingModule {}
