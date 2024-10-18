import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableCountryPageComponent } from './table-country-page/table-country-page.component';
import { CountryPageComponent } from './country-page.component';

const routes: Routes = [
  {
    path: '',
    component: CountryPageComponent,
    children: [
      {
        path: 'list',
        component: TableCountryPageComponent,
      },{
        path: 'list',
        component: TableCountryPageComponent,
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
export class CountryPageRoutingModule {}
