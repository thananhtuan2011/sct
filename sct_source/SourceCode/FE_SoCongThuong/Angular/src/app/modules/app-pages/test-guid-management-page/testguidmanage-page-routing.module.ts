import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTestGuidManagementPageComponent } from './table-testguidmanagement-page/table-page.component';
import { TestGuidManagementPageComponent } from './testguidmanage-page.component';

const routes: Routes = [
  {
    path: '',
    component: TestGuidManagementPageComponent,
    children: [
      {
        path: 'list',
        component: TableTestGuidManagementPageComponent,
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
export class TestGuidManagementPageRoutingModule {}
