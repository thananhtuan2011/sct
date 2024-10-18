import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MnFvsPProducingAlcoholPageComponent } from './mnfvspproducingalcohol-page.component';

import { TableCateRPSAncolForFactoryPageComponent } from './table-caterpsancolforfactory-page/table-caterpsancolforfactory-page.component';
import { TableCateRPSoldAncolPageComponent } from './table-caterpsoldancol-page/table-caterpsoldancol-page.component';
import { TableCateRPProduceIndustlAncolPageComponent } from './table-caterpproduceindustlancol-page/table-caterpproduceindustlancol-page.component';
import { TableCateRPTurnOverIndustAncolPageComponent } from './table-caterpturnoverindustancol-page/table-caterpturnoverindustancol-page.component';
import { TableCateRPPCrafttAncolForEconomicPageComponent } from './table-caterppcrafttancolforeconomic-page/table-page.component';


const routes: Routes = [
  {
    path: '',
    component: MnFvsPProducingAlcoholPageComponent,
    children: [
      {
        path: 'CateRPSAncolForFactory', //Báo cáo tình hình rượu thủ công bán cho cơ sở có giấy phép sản xuất rượu
        component: TableCateRPSAncolForFactoryPageComponent,
      },
      {
        path: 'CateRPPCrafttAncolForEconomic', //Báo cáo tình hình sản xuất rượu thủ công nhằm mục đích kinh doanh
        component: TableCateRPPCrafttAncolForEconomicPageComponent,
      },
      {
        path: 'CateRPSoldAncol', //Báo cáo tình hình bán lẻ rượu
        component: TableCateRPSoldAncolPageComponent,
      },
      {
        path: 'CateRPProduceIndustlAncol', //Báo cáo tình hình sản xuất rượu công nghiệp (quy mô dưới 3 triệu lít/năm)
        component: TableCateRPProduceIndustlAncolPageComponent,
      },
      {
        path: 'CateRPTurnOverIndustAncol', //Báo cáo tình hình kinh doanh rượu công nghiệp (quy mô dưới 3 triệu lít/năm)
        component: TableCateRPTurnOverIndustAncolPageComponent,
      },
      { path: '', redirectTo: 'CateRPSAncolForFactory', pathMatch: 'full' },
      { path: '**', redirectTo: 'CateRPSAncolForFactory', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MnFvsPProducingAlcoholPageRoutingModule {}
