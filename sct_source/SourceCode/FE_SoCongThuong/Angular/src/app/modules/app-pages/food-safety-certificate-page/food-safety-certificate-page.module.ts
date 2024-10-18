import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
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
import { MatMenuModule } from '@angular/material/menu';

import { FoodSafetyCertificatePageService } from './_services/food-safety-certificate-page.service';
import { FoodSafetyCertificatePageRoutingModule } from './food-safety-certificate-page-routing.module';

import { FoodSafetyCertificatePageComponent } from './food-safety-certificate-page.component';
import { TableFoodSafetyCertificatePageComponent } from './table-food-safety-certificate-page/table-page.component';
import { EditFoodSafetyCertificateModalComponent } from './table-food-safety-certificate-page/components/edit-food-safety-certificate-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { MatRadioModule } from '@angular/material/radio';
import { AddTypeOfProductionModalComponent } from './table-food-safety-certificate-page/components/add-type-of-production-modal/add-modal.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { ViewCertificateModalComponent } from './table-food-safety-certificate-page/components/view-certificate-modal/view-modal.component';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';

@NgModule({
  declarations: [
    FoodSafetyCertificatePageComponent,
    TableFoodSafetyCertificatePageComponent,
    EditFoodSafetyCertificateModalComponent,
    AddTypeOfProductionModalComponent,
    ViewCertificateModalComponent
  ],
  providers: [
    FoodSafetyCertificatePageService
	],
  imports: [
    CommonModule,
    FoodSafetyCertificatePageRoutingModule,
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
    MatCheckboxModule,
    CRUDTableModule,
    SelectCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule,
    DateTimePickerModule,
    MatRadioModule,
    MatIconModule,
    MatButtonModule,
    NgxDocViewerModule,
    NgxDropzoneModule,
    DndDirectiveModule,
  ]
})
export class FoodSafetyCertificatePageModule {}
