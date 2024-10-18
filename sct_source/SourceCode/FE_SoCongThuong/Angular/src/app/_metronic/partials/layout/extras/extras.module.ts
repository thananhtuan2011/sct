import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';
import {InlineSVGModule} from 'ng-inline-svg-2';
import {NotificationsInnerComponent} from './dropdown-inner/notifications-inner/notifications-inner.component';
import {QuickLinksInnerComponent} from './dropdown-inner/quick-links-inner/quick-links-inner.component';
import {UserInnerComponent} from './dropdown-inner/user-inner/user-inner.component';
import {LayoutScrollTopComponent} from './scroll-top/scroll-top.component';
import {TranslationModule} from '../../../../modules/i18n';
import {SearchResultInnerComponent} from "./dropdown-inner/search-result-inner/search-result-inner.component";
import {NgbTooltipModule} from "@ng-bootstrap/ng-bootstrap";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { UserService } from './../extras/dropdown-inner/user-inner/_services/user.service';
import { ChangePasswordModalComponent } from './../extras/dropdown-inner/user-inner/user-page/components/change-password-modal/change-password-modal.component';
import { PasswordStrengthModule } from 'src/app/_metronic/shared/components/password-strength/password-strength.module';
@NgModule({
  declarations: [
    NotificationsInnerComponent,
    QuickLinksInnerComponent,
    SearchResultInnerComponent,
    UserInnerComponent,
    LayoutScrollTopComponent,
    ChangePasswordModalComponent,
  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, InlineSVGModule, RouterModule, TranslationModule, NgbTooltipModule, PasswordStrengthModule],
  exports: [
    NotificationsInnerComponent,
    QuickLinksInnerComponent,
    SearchResultInnerComponent,
    UserInnerComponent,
    LayoutScrollTopComponent,
    NgbModule,
  ],
})
export class ExtrasModule {
}
