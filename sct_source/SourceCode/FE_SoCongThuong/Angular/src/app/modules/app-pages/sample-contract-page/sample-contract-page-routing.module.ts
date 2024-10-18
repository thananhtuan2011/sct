import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {TableSampleContractPageComponent} from './table-sample-contract-page/table-page.component';
import {SampleContractPageComponent} from './sample-contract-page.component';

const routes: Routes = [
    {
        path: '',
        component: SampleContractPageComponent,
        children: [
            {
                path: 'list',
                component: TableSampleContractPageComponent
            }, {
                path: '',
                redirectTo: 'list',
                pathMatch: 'full'
            }, {
                path: '**',
                redirectTo: 'list',
                pathMatch: 'full'
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class SampleContractPageRoutingModule {}