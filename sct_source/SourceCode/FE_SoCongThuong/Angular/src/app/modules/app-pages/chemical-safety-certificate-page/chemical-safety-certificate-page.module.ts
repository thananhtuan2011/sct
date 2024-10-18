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

import { ChemicalSafetyCertificatePageService } from './_services/chemical-safety-certificate-page.service';
import { ChemicalSafetyCertificatePageRoutingModule } from './chemical-safety-certificate-page-routing.module';

import { ChemicalSafetyCertificatePageComponent } from './chemical-safety-certificate-page.component';
import { TableChemicalSafetyCertificatePageComponent } from './table-chemical-safety-certificate-page/table-page.component';
import { EditChemicalSafetyCertificateModalComponent } from './table-chemical-safety-certificate-page/components/edit-chemical-safety-certificate-modal/edit-modal.component';

import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { MatRadioModule } from '@angular/material/radio';
import { AddChemicalInfoModalComponent } from './table-chemical-safety-certificate-page/components/add-chemical-info-modal/add-modal.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { ViewCertificateModalComponent } from './table-chemical-safety-certificate-page/components/view-certificate-modal/view-modal.component';
import { NgxDropzoneModule } from 'ngx-dropzone';
import { DndDirectiveModule } from 'src/app/_metronic/shared/Upload/dnd/dnd.directive.module';

@NgModule({
  declarations: [
    ChemicalSafetyCertificatePageComponent,
    TableChemicalSafetyCertificatePageComponent,
    EditChemicalSafetyCertificateModalComponent,
    AddChemicalInfoModalComponent,
    ViewCertificateModalComponent
  ],
  providers: [
    ChemicalSafetyCertificatePageService
	],
  imports: [
    CommonModule,
    ChemicalSafetyCertificatePageRoutingModule,
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
export class ChemicalSafetyCertificatePageModule {}
