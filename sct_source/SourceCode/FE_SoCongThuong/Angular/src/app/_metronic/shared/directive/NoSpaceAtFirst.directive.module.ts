import { NgModule } from '@angular/core';
import { NoSpaceAtFirstDirective } from './NoSpaceAtFirst.directive';

@NgModule({
  declarations: [NoSpaceAtFirstDirective],
  exports: [NoSpaceAtFirstDirective]
})

export class NoSpaceAtFirstDirectiveModule { }