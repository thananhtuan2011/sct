import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableBusinessMultiLevelPageComponent } from './table-business-multi-level-page/table-page.component';
import { BusinessMultiLevelPageComponent } from './business-multi-level-page.component';

const routes: Routes = [
  {
    path: '',
    component: BusinessMultiLevelPageComponent,
    children: [
      {
        path: 'list',
        component: TableBusinessMultiLevelPageComponent,
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
export class BusinessMultiLevelPageRoutingModule {}
