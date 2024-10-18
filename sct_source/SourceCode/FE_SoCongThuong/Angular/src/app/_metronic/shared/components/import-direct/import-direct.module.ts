import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ImportDirectComponent } from './import-direct.component';
@NgModule({
    imports: [
        FormsModule,
        NgbModule,
        ReactiveFormsModule,
        CommonModule,
    ],
    declarations: [ImportDirectComponent],
    exports: [ImportDirectComponent],
})

export class ImportDirectModule {}
