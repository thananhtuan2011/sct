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
import { InterCommercePageService } from './_services/inter-commerce-page.service';
import { InterCommerceRoutingModule } from './inter-commerce-page-routing.module';
import { EditInterCommerceModalComponent } from './inter-commerce-page/components/edit-inter-commerce-modal/edit-inter-commerce-modal.component';
import { TableInterCommerceModalComponent } from './inter-commerce-page/table-inter-commerce-page.component';
import { InterCommerceComponent } from './inter-commerce-page.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { DatePickerCustomModule } from '../../../_metronic/shared/components/date-picker/date-picker-custom.module';

@NgModule({
  declarations: [
    EditInterCommerceModalComponent,
    TableInterCommerceModalComponent,
    InterCommerceComponent
  ],
  providers: [
    InterCommercePageService
	],
  imports: [
    CommonModule,
    InterCommerceRoutingModule,
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
    ImportDirectModule,
    DatePickerCustomModule
  ]
})
export class InterCommerceModule {} 