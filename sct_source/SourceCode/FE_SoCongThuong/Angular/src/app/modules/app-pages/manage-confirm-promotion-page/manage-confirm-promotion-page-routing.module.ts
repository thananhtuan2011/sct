import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableManageConfirmPromotionPageComponent } from './table-manageconfirmpromotion-page/table-manage-confirm-promotion-page.component';
import { ManageConfirmPromotionPageComponent } from './manage-confirm-promotion-page.component';

const routes: Routes = [
  {
    path: '',
    component: ManageConfirmPromotionPageComponent,
    children: [
      {
        path: 'list',
        component: TableManageConfirmPromotionPageComponent,
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
export class ManageConfirmPromotionPageRoutingModule {}
