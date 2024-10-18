import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from './../../../_metronic/shared/crud-table/crud-table.module';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table'
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { NgbDateAdapter, NgbDateParserFormatter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ProductOcopPageRoutingModule } from './product-ocop-page-routing.module';
import { TableProductOcopPageComponent } from './table-product-ocop-page/table-page.component';
import { ProductOcopPageComponent } from './product-ocop-page.component';
import { ProductOcopPageService } from './_services/product-ocop-page.service';
import { EditProductOcopModalComponent } from './table-product-ocop-page/components/edit-product-ocop-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NgxStarRatingModule } from 'src/app/_metronic/shared/components/ngx-star-rating/ngx-star-rating.module'
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';
import { progressModule } from 'src/app/_metronic/shared/components/progress-upload/progress.component.module';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    ProductOcopPageComponent,
    TableProductOcopPageComponent,
    EditProductOcopModalComponent
  ],
  providers: [
    ProductOcopPageService,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    ProductOcopPageRoutingModule,
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
    NgxStarRatingModule,
    NgxDropzoneModule,
    DndDirectiveModule,
    progressModule,
    DatePickerCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class ProductOcopPageModule {}