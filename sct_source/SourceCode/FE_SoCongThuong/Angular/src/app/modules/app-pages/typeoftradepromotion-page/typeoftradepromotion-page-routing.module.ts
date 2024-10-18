import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableTypeOfTradePromotionPageComponent } from './table-typeoftradepromotion-page/table-typeoftradepromotion-page.component';
import { TypeOfTradePromotionPageComponent } from './typeoftradepromotion-page.component';

const routes: Routes = [
  {
    path: '',
    component: TypeOfTradePromotionPageComponent,
    children: [
      {
        path: 'list',
        component: TableTypeOfTradePromotionPageComponent,
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
export class TypeOfTradePromotionPageRoutingModule {}
