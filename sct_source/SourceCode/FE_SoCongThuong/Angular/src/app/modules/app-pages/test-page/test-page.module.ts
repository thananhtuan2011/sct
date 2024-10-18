import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TestPageRoutingModule } from './test-page-routing.module';
import { TestPageComponent } from './test-page.component';
import { TestPageListComponent } from './test-page-list/test-page-list.component';
import { InlineSVGModule } from 'ng-inline-svg-2';
import { MatTableModule } from '@angular/material/table' 
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { TestPageService } from './test-page.service';
import { TestPageModalComponent } from './test-page-modal/test-page-modal.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
@NgModule({
  declarations: [
    TestPageComponent,
    TestPageListComponent,
    TestPageModalComponent,
  ],
  providers: [
		TestPageService
	],
  imports: [
    CommonModule, 
    TestPageRoutingModule,
    InlineSVGModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    NgbModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class TestPageModule {}
