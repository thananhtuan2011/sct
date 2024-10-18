import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TableStatisticalMultiLevelSalesPageComponent } from './table-statistical-multi-level-sales-page/table-page.component';
import { StatisticalMultiLevelSalesPageComponent } from './statistical-multi-level-sales-page.component';

const routes: Routes = [
    {
        path: '',
        component: StatisticalMultiLevelSalesPageComponent,
        children: [
            {
                path: 'list',
                component: TableStatisticalMultiLevelSalesPageComponent,
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
export class StatisticalMultiLevelSalesPageRoutingModule { }
