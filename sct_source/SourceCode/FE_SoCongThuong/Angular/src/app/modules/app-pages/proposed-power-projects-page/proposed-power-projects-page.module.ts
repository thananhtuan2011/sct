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
import { ProposedPowerProjectsPageRoutingModule } from './proposed-power-projects-page-routing.module';
import { TableProposedPowerProjectsPageComponent } from './table-proposed-power-projects-page/table-page.component';
import { ProposedPowerProjectsPageComponent } from './proposed-power-projects-page.component';
import { ProposedPowerProjectsPageService } from './_services/proposed-power-projects-page.service';
import { EditProposedPowerProjectsModalComponent } from './table-proposed-power-projects-page/components/edit-proposed-power-projects-modal/edit-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from 'src/app/_metronic/shared/components/date-picker/date-picker-custom.module';

@NgModule({
  declarations: [
    ProposedPowerProjectsPageComponent,
    TableProposedPowerProjectsPageComponent,
    EditProposedPowerProjectsModalComponent
  ],
  providers: [
    ProposedPowerProjectsPageService
	],
  imports: [
    CommonModule,
    ProposedPowerProjectsPageRoutingModule,
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
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    DatePickerCustomModule
  ]
})
export class ProposedPowerProjectsPageModule {}