import { TreePhanQuyenModule } from './../../../_metronic/shared/components/tree-phan-quyen/tree-phan-quyen.module';
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
import { GroupUserService } from './_services/group-user.service';
import {MatMenuModule} from '@angular/material/menu';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { TableGroupUserComponent } from './table-group-user/table-group-user.component';
import { GroupUserPageComponent } from './group-user-page.component';
import { GroupUserPageRoutingModule } from './group-user-page-routing.module';
import { EditGroupUserModalComponent } from './table-group-user/components/edit-group-user-modal/edit-group-user-modal.component';
import { EditPermissionGroupUserModalComponent } from './table-group-user/components/edit-permission-group-user-modal/edit-permission-group-user-modal.component';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';

@NgModule({
  declarations: [
    GroupUserPageComponent,
    EditGroupUserModalComponent,
    TableGroupUserComponent,
    EditPermissionGroupUserModalComponent
  ],
  providers: [
    GroupUserService
	],
  imports: [
    CommonModule,
    GroupUserPageRoutingModule,
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
    TreePhanQuyenModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule
  ]
})
export class GroupUserPageModule {}
