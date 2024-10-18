import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginatorComponent } from './components/paginator/paginator.component';
import { NgPagination } from './components/paginator/ng-pagination/ng-pagination.component';
import { FormsModule } from '@angular/forms';
import { SortIconComponent } from './components/sort-icon/sort-icon.component';
//import { InlineSVGModule } from 'ng-inline-svg';
import { TranslateModule } from '@ngx-translate/core';
import { BreadcrumbComponent } from './components/breadcrumb/breadcrumb.component';
import { RouterModule } from '@angular/router';
@NgModule({
  declarations: [
    PaginatorComponent,
    NgPagination,
    SortIconComponent,
    BreadcrumbComponent
  ],
  imports: [
    TranslateModule,
    CommonModule,
    FormsModule,
    RouterModule
    //InlineSVGModule
  ],
  exports: [PaginatorComponent,
    NgPagination,
    SortIconComponent,
    BreadcrumbComponent
  ],
})
export class CRUDTableModule { }
