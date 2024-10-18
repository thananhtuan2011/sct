import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConsumerServiceRevenueComponent } from './consumer-service-revenue-page.component';
import { TableConsumerServiceRevenueModalComponent } from './consumer-service-revenue-page/table-page.component';

const routes: Routes = [
  {
    path: '',
    component: ConsumerServiceRevenueComponent,
    children: [
      {
        path: 'list',
        component: TableConsumerServiceRevenueModalComponent,
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
export class ConsumerServiceRevenueRoutingModule {}
