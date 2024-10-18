import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ControlsPageComponent } from './controls-page.component';
import { ControlsCustomPageComponent } from './controls-custom-page/controls-custom-page.component';
const routes: Routes = [
  {
    path: '',
    component: ControlsPageComponent,
    children: [
      {
        path: 'list',
        component: ControlsCustomPageComponent,
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
