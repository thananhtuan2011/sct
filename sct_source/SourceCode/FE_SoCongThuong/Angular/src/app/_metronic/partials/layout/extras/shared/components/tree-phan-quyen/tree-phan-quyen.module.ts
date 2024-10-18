import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TreePhanQuyenNodeComponent } from './tree-phan-quyen-node/tree-phan-quyen-node.component';
import { EventBusService } from './_services/event-bus.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { TreePhanQuyenComponent } from './tree-phan-quyen.component';



@NgModule({
  declarations: [
    TreePhanQuyenNodeComponent,
    TreePhanQuyenComponent
  ],
  imports: [
    CommonModule,
    MatCheckboxModule
  ],
  entryComponents:[
    TreePhanQuyenNodeComponent,
    TreePhanQuyenComponent
  ],
  exports:[
    TreePhanQuyenNodeComponent,
    TreePhanQuyenComponent
  ],
	providers: [
    EventBusService
  ]
})
export class TreePhanQuyenModule { }
