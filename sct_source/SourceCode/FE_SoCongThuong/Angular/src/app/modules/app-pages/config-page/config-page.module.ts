import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { GroupConfigPageRoutingModule } from './config-page-routing.module';
import { TableGroupConfigPageComponent } from './table-config-page/table-page.component';
import { GroupConfigPageComponent } from './config-page.component';
import { GroupConfigPageService } from './_services/config-group-page.service';
import { EditGroupConfigModalComponent } from './table-config-page/components/edit-group-config-modal/edit-modal.component';
import { EditConfigModalComponent } from './table-config-page/components/edit-config-modal/edit-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    GroupConfigPageComponent,
    TableGroupConfigPageComponent,
    EditGroupConfigModalComponent,
    EditConfigModalComponent
  ],
  providers: [
    GroupConfigPageService,
	],
  imports: [
    CommonModule,
    GroupConfigPageRoutingModule,
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
    MatButtonModule,
    MatCheckboxModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class GroupConfigPageModule {}