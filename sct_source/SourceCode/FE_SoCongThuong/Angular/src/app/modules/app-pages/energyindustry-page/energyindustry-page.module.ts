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
import { EnergyIndustryPageRoutingModule } from './energyindustry-page-routing.module';
import { TableEnergyIndustryPageComponent } from './table-energyindustry-page/table-energyindustry-page.component';
import { EnergyIndustryPageComponent } from './energyindustry-page.component';
import { EnergyIndustryPageService } from './_services/energyindustry-page.service';
import { EditEnergyIndustryModalComponent } from './table-energyindustry-page/components/edit-energyindustry-modal/edit-energyindustry-modal.component';
import { MatMenuModule } from '@angular/material/menu';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    EnergyIndustryPageComponent,
    TableEnergyIndustryPageComponent,
    EditEnergyIndustryModalComponent
  ],
  providers: [
    EnergyIndustryPageService
	],
  imports: [
    CommonModule,
    EnergyIndustryPageRoutingModule,
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
    ImportDirectModule
  ]
})
export class EnergyIndustryPageModule {}