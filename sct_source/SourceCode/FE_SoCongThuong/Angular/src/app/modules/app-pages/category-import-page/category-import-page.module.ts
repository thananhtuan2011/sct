import { SafeHtmlPipe } from './../../../_metronic/shared/pipe/SafeHtmlPipe/SafeHtmlPipe';
import { SearchPipeModule } from './../../../_metronic/shared/pipe/filter-pipe/filter.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import {MatMenuModule} from '@angular/material/menu';
import { CategoryImportPageRoutingModule } from './category-import-page-routing.module';
import { CategoryImportPageComponent } from './category-import-page.component';
import { TableCategoryImportPageComponent } from './table-category-import-page/table-category-import-page.component';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import {MatIconModule} from '@angular/material/icon';
import { CategoryImportService } from './_services/category-import.service';
import {MatListModule} from '@angular/material/list';
import { ViewCategoryImportModalComponent } from './table-category-import-page/components/view-category-import-modal/view-category-import-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    CategoryImportPageComponent,
    TableCategoryImportPageComponent,
    ViewCategoryImportModalComponent,
    SafeHtmlPipe
  ],
  providers: [
    CategoryImportService
	],
  imports: [
    CommonModule,
    CategoryImportPageRoutingModule,
    InlineSVGModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatMenuModule,
    CRUDTableModule,
    SelectCustomModule,
    MatIconModule,
    MatListModule,
    SearchPipeModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class CategoryImportPageModule {}
