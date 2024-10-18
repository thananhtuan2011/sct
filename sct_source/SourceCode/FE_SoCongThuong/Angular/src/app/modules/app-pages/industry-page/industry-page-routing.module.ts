import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { IndustryPageComponent } from './industry-page.component';
import { TableIndustryPageComponent } from './table-industry-page/table-industry-page.component';
const routes: Routes = [
  {
    path: '',
    component: IndustryPageComponent,
    children: [
      {
        path: 'list',
        component: TableIndustryPageComponent,
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
export class CustomPageRoutingModule {}
