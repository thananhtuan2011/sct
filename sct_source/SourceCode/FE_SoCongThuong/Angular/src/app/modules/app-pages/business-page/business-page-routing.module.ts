import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableBusinessPageComponent } from './table-business-page/table-business-page.component';
import { InfoBusinessPageComponent } from './table-business-page/components/info-business-page/info-business-page.component'
import { BusinessPageComponent } from './business-page.component';

const routes: Routes = [
  {
    path: '',
    component: BusinessPageComponent,
    children: [
      {
        path: 'user/:id',
        component: InfoBusinessPageComponent,
      },
      {
        path: 'list',
        component: TableBusinessPageComponent,
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
export class BusinessPageRoutingModule {}
