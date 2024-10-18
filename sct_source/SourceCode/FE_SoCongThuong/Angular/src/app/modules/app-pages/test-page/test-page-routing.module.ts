import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestPageListComponent } from './test-page-list/test-page-list.component';
import { TestPageComponent } from './test-page.component';

const routes: Routes = [
  {
    path: '',
    component: TestPageComponent,
    children: [
      
      {
        path: 'list',
        component: TestPageListComponent,
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
export class TestPageRoutingModule {}
