import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { CRUDTableModule } from '../../../_metronic/shared/crud-table/crud-table.module';
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
import { RegulationConformityAMPageRoutingModule } from './reg-conform-AM-page-routing.module';
import { TableRegulationConformityAMPageComponent } from './table-regulation-conformity-AM-page/table-page.component';
import { RegulationConformityAMPageComponent } from './reg-conform-AM-page.component';
import { RegulationConformityAMPageService } from './_services/regulation-conformity-AM.service';
import { EditRegulationConformityAMModalComponent } from './table-regulation-conformity-AM-page/components/edit-regulation-conformity-AM-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { DateTimePickerModule } from 'src/app/_metronic/shared/components/date-time-picker/date-time-picker.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    RegulationConformityAMPageComponent,
    TableRegulationConformityAMPageComponent,
    EditRegulationConformityAMModalComponent,
  ],
  providers: [
    RegulationConformityAMPageService,
	],
  imports: [
    CommonModule,
    RegulationConformityAMPageRoutingModule,
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
    DateTimePickerModule,
    DatePickerCustomModule,
    MatButtonModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class RegulationConformityAMPageModule {}