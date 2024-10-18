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
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SearchPipeModule } from 'src/app/_metronic/shared/pipe/filter-pipe/filter.module';
import { CigaretteBusinessComponent } from './cigarette-bus-page.component';
import { CigaretteBusinessRoutingModule } from './cigarette-bus-page-routing.module';
import { TableCigeratteBusinessModalComponent } from './cigarette-bus-page/table-cigarette-bus-page.component';
import { EditCigaretteBusinessModalComponent } from './cigarette-bus-page/components/edit-cigarette-bus-modal/edit-cigarette-bus-modal.component';
import { CigaretteBusinessPageService } from './_services/cigarette-bus-page.service';
import { CustomAdapter, CustomDateParserFormatter } from 'src/app/_metronic/shared/pipe/CustomNgbDate/CustomNgbDate';
import { InfoCigaretteBusinessModalComponent } from './cigarette-bus-page/components/info-page-modal/info-page-modal.component';
import { InfoCigaretteBusinessDetailModalComponent } from './cigarette-bus-page/components/info-detail-modal/info-detail-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    EditCigaretteBusinessModalComponent,
    TableCigeratteBusinessModalComponent,
    CigaretteBusinessComponent,
    InfoCigaretteBusinessModalComponent,
    InfoCigaretteBusinessDetailModalComponent,
  ],
  providers: [
    CigaretteBusinessPageService,
    { provide: NgbDateAdapter, useClass: CustomAdapter },
		{ provide: NgbDateParserFormatter, useClass: CustomDateParserFormatter },
	],
  imports: [
    CommonModule,
    CigaretteBusinessRoutingModule,
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
    MatListModule,
    ScrollingModule,
    MatDividerModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatCheckboxModule,
    SearchPipeModule,
    MatDatepickerModule,
    MatMomentDateModule,
    MatDatepickerModule,
    MatNativeDateModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class CigaretteBusinessModule {}