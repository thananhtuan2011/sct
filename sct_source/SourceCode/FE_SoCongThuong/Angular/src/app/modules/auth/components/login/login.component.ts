import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription, Observable } from 'rxjs';
import { first } from 'rxjs/operators';
import { UserModel, UserLoginFailModel } from '../../models/user.model';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { RecaptchaComponent } from 'ng-recaptcha';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  token: string | undefined;
  @ViewChild(RecaptchaComponent) recaptcha: RecaptchaComponent;

  HiddenCaptcha: boolean = true;
  loginFail: number = 0;
  timeLock: Date | null;
  notification: string = "Đăng nhập thất bại, vui Lòng thử lại.";
  loginForm: FormGroup;
  hasError: boolean;
  returnUrl: string;
  show: boolean = false;
  isLoading$: Observable<boolean>;

  // private fields
  private unsubscribe: Subscription[] = []; // Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router,
  ) {
    this.isLoading$ = this.authService.isLoading$;
    // redirect to home if already logged in
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    };
    this.token = undefined;
  }

  ngOnInit(): void {
    this.initForm();
    // get return url from route parameters or default to '/'
    this.returnUrl =
      this.route.snapshot.queryParams['returnUrl'.toString()] || '/';
  }

  // convenience getter for easy access to form fields
  get f() {
    return this.loginForm.controls;
  }

  initForm() {
    this.loginForm = this.fb.group({
      email: [
        '',
        Validators.compose([
          Validators.required,
          //Validators.email,
          // Validators.minLength(3),
          // Validators.maxLength(320),
        ]),
      ],
      password: [
        '',
        Validators.compose([
          Validators.required,
          // Validators.minLength(3),
          // Validators.maxLength(100),
        ]),
      ]
    });
    this.show = true;
  }

  submit() {
    this.hasError = false;
    const loginSubscr = this.authService
      .login(this.f.email.value, this.f.password.value, this.token ?? "")
      .pipe(
      // first()
    )
      .subscribe((user: UserModel | UserLoginFailModel | undefined) => {
        // if (user) {
        //   if (!(user as UserLoginFailModel).countLoginFail) {
        //     this.router.navigate([this.returnUrl]);
        //   }
        //   else {
        //     this.handleLoginFail((user as UserLoginFailModel).countLoginFail, (user as UserLoginFailModel).timeLock)
        //     this.hasError = true;
        //   }
        // } else {
        //   this.hasError = true;
        // }
        if (user) {
          var check = Object.keys(user).length;
          if (check == 3) {
            const data = user as UserLoginFailModel;
            this.handleLoginFail(data.countLoginFail, data.timeLock);
            this.hasError = true;
          } else {
            this.router.navigate([this.returnUrl]);
          }
        } else {
          this.hasError = true;
        }
      },
        () => {
          this.hasError = true;
        });
    this.unsubscribe.push(loginSubscr);
  }

  handleLoginFail(count: number, timeLock: Date | null) {
    var notification = "Đăng nhập thất bại, vui Lòng thử lại.";
    if (count > 1 && count < 4) {
      this.recaptcha.reset();
      this.HiddenCaptcha = false;
      notification = `Bạn đã đăng nhập sai ${count} lần, hãy xác minh Captcha và thử lại.`;
    } else if (count > 3 && count < 8) {
      this.HiddenCaptcha = true;
      const time = this.calculatorTime(timeLock);
      if (time === null) {
        notification = `Bạn đã đăng nhập sai ${count} lần, vui lòng thử lại.`;
      } else {
        notification = `Bạn đã đăng nhập sai ${count} lần, vui lòng thử lại sau ${time}`;
      }
    } else if (count === 8) {
      this.HiddenCaptcha = true;
      notification = "Tài khoản bạn đã bị khoá vĩnh viễn, vui lòng liên hệ quản trị viên để được hổ trợ.";
    }
    this.notification = notification;
  }

  // calculatorTime(time: Date | null): string | null {
  //   this.time = time;
  //   if (time) {
  //     const now = new Date().getTime();
  //     const timeLock = new Date(time).getTime();
  //     const msDiff = timeLock - now;
  //     this.msDiff = msDiff;
  //     if (msDiff > 0) {
  //       // 1 giây = 1000ms
  //       const seconds = Math.floor(msDiff / 1000);
  //       const minutes = Math.floor(seconds / 60);
  //       const hours = Math.floor(minutes / 60);
  //       const remainingMinutes = minutes % 60;
  //       const remainingSeconds = seconds % 60;
  //       return hours > 0
  //         ? `${hours} giờ ${remainingMinutes} phút ${remainingSeconds} giây.`
  //         : remainingMinutes > 0
  //           ? `${remainingMinutes} phút ${remainingSeconds} giây.`
  //           : `${remainingSeconds} giây.`;
  //     } else {
  //       return null
  //     }
  //   }
  //   return null;
  // }

  calculatorTime(time: Date | null): string | null {
    if (time) {
      const now = moment.utc();
      const timeLock = moment.utc(time);
      const duration = moment.duration(timeLock.diff(now));
      if (duration.asMilliseconds() > 0) {
        return duration.hours() > 0
          ? `${duration.hours()} giờ ${duration.minutes()} phút ${duration.seconds()} giây.`
          : duration.minutes() > 0
            ? `${duration.minutes()} phút ${duration.seconds()} giây.`
            : `${duration.seconds()} giây.`;
      } else {
        return null;
      }
    }
    return null;
  }

  ngOnDestroy() {
    this.unsubscribe.forEach((sb) => sb.unsubscribe());
  }
}
