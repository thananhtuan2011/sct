import { Component, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
// import { TranslationService } from '../../../../../../modules/i18n';
import { AuthService, UserModel, UserType } from '../../modules/auth';
@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
})
export class AccountComponent implements OnInit {
  user$: Observable<UserModel | undefined>;
  constructor(
    public auth: AuthService,
  ) {

  }

  ngOnInit(): void {
    this.user$ = this.auth.currentUserSubject.asObservable();
  }
}
