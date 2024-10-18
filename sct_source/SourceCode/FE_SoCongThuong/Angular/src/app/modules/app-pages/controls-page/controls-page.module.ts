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
import { MatRadioModule } from '@angular/material/radio';//radio control
import { MatSelectModule } from '@angular/material/select';//Select controls
import { MatCheckboxModule } from '@angular/material/checkbox';//checkbox controls
import { MatIconModule } from '@angular/material/icon';//icon controls
import { MatAutocompleteModule } from '@angular/material/autocomplete';//AutoComplete controls
import { MatDatepickerModule } from '@angular/material/datepicker';//datepicker controls
import { MatTabsModule } from '@angular/material/tabs';//icon controls
import { MatExpansionModule } from '@angular/material/expansion';//icon controls
import { MatSlideToggleModule } from '@angular/material/slide-toggle';//icon controls
import { CustomPageRoutingModule } from './controls-page-routing.module';
import { ControlsCustomPageComponent } from './controls-custom-page/controls-custom-page.component';
import { ControlsPageComponent } from './controls-page.component';
import { CustomPageService } from './_services/custom-page.service';
import { EditCustomModalComponent } from './controls-custom-page/components/edit-custom-modal/edit-custom-modal.component';
import {MatMenuModule} from '@angular/material/menu';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import {MatTooltipModule} from '@angular/material/tooltip';
import { MatNativeDateModule } from '@angular/material/core';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';//Use format date
import { MomentDateAdapter } from '@angular/material-moment-adapter';//Use format date
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
const MY_DATE_FORMAT = {
  parse: {
    dateInput: 'DD/MM/YYYY', // this is how your date will be parsed from Input
  },
  display: {
    dateInput: 'DD/MM/YYYY', // this is how your date will get displayed on the Input
    monthYearLabel: 'MMMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
  }
};//Format Date

@NgModule({
  declarations: [
    ControlsPageComponent,
    ControlsCustomPageComponent,
    EditCustomModalComponent
  ],
  providers: [
    CustomPageService,
    MatDatepickerModule,
    MatNativeDateModule,
    //Begin format date (dd/MM/YYYY)
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMAT },
    //End format date (dd/MM/YYYY)
	],
  imports: [
    CommonModule,
    CustomPageRoutingModule,
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
    MatRadioModule,//radio control
    MatSelectModule,//Select control
    NgxMatSelectSearchModule,
    MatCheckboxModule,//Checkbox,
    MatIconModule,//icon,
    MatAutocompleteModule,//Autocomplete,
    MatTooltipModule,
    MatDatepickerModule,//DatePicker
    MatTabsModule,//tabs control
    MatExpansionModule,//Panels ,
    MatSlideToggleModule,//Slide toggle
    MatNativeDateModule,//date
    SelectCustomModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class CustomPageModule {}
