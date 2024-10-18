import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SelectCustomComponent } from './select-custom.component';
@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [SelectCustomComponent],
  exports: [SelectCustomComponent]
})
export class SelectCustomModule { }
