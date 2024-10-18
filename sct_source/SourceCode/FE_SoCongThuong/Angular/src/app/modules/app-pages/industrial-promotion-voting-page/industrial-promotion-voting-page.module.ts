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
import { ResultsIndustrialPromotionVotingPageRoutingModule } from './industrial-promotion-voting-page-routing.module';
import { TableResultsIndustrialPromotionVotingPageComponent } from './table-industrial-promotion-voting-page/table-page.component';
import { ResultsIndustrialPromotionVotingPageComponent } from './industrial-promotion-voting-page.component';
import { ResultsIndustrialPromotionVotingPageService } from './_services/results-industrial-promotion-voting-page.service';
import { EditResultsIndustrialPromotionVotingModalComponent } from './table-industrial-promotion-voting-page/components/edit-industrial-promotion-voting-modal/edit-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
@NgModule({
  declarations: [
    ResultsIndustrialPromotionVotingPageComponent,
    TableResultsIndustrialPromotionVotingPageComponent,
    EditResultsIndustrialPromotionVotingModalComponent
  ],
  providers: [
    ResultsIndustrialPromotionVotingPageService
	],
  imports: [
    CommonModule,
    ResultsIndustrialPromotionVotingPageRoutingModule,
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
    MatRadioModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class ResultsIndustrialPromotionVotingPageModule {}
