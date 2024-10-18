import { Component, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
// import { TranslationService } from '../../../../../../modules/i18n';
import { AuthService, UserType } from '../../../modules/auth';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserModel } from 'src/app/modules/auth';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// import { UserService } from '../../../../layout/extras/dropdown-inner/user-inner/_services/user.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  user$: Observable<UserModel | undefined>;
  userCurrent: UserModel | undefined;
  constructor(public auth: AuthService,) {}

  ngOnInit(): void {
    this.user$ = this.auth.currentUserSubject.asObservable();
    console.log(this.user$);
  }
}
