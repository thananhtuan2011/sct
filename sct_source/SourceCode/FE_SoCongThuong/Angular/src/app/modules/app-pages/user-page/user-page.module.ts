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
import { UserPageRoutingModule } from './user-page-routing.module';
import { UserPageComponent } from './user-page.component';
import { UserService } from './_services/user.service';
import {MatMenuModule} from '@angular/material/menu';
import { EditUserModalComponent } from './user-page/components/edit-user-modal/edit-user-modal.component';
import { TableUserComponent } from './user-page/table-user.component';
import { SelectCustomModule } from 'src/app/_metronic/shared/components/select-custom/select-custom.module';
import { ChangePasswordModalComponent } from './user-page/components/change-password-modal/change-password-modal.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatButtonModule } from '@angular/material/button';
import { NoSpaceAtFirstDirectiveModule } from 'src/app/_metronic/shared/directive/NoSpaceAtFirst.directive.module';
import { ImportDirectModule } from 'src/app/_metronic/shared/components/import-direct/import-direct.module';
import { PasswordStrengthModule } from 'src/app/_metronic/shared/components/password-strength/password-strength.module';
import { TreeUserComponent } from './user-page/components/tree-user-modal/tree-user-modal.component';
import { MatIconModule } from '@angular/material/icon';
import { MatTreeModule } from '@angular/material/tree';
import { CdkTreeModule } from '@angular/cdk/tree';

@NgModule({
  declarations: [
    UserPageComponent,
    TableUserComponent,
    EditUserModalComponent,
    ChangePasswordModalComponent,
    TreeUserComponent
  ],
  providers: [
    UserService
	],
  imports: [
    CommonModule,
    UserPageRoutingModule,
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
    MatCheckboxModule,
    MatRadioModule,
    MatButtonModule,
    MatTreeModule,
    MatIconModule,
    CdkTreeModule,
    NoSpaceAtFirstDirectiveModule,
    ImportDirectModule,
    PasswordStrengthModule
  ]
})
export class UserPageModule {}
