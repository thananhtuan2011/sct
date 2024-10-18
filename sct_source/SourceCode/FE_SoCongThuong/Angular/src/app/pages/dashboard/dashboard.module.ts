import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { ModalsModule, WidgetsModule } from '../../_metronic/partials';
import { NgChartsModule } from  'ng2-charts'
import { DashboardService } from './_services/dashboard.service';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [DashboardComponent],
  providers:[
    DashboardService
  ],
  imports: [
    RouterModule.forChild([
      {
        path: '',
        component: DashboardComponent,
      },
    ]),
    //WidgetsModule,
    FormsModule,
    ReactiveFormsModule,
    ModalsModule,
    CommonModule,
    NgChartsModule,
    SelectCustomModule,
  ],
})
export class DashboardModule {}
