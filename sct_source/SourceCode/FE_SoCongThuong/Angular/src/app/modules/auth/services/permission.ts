
import { Router, RouterStateSnapshot, ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanLoad, Route } from '@angular/router';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class Permission implements CanActivate, CanActivateChild, CanLoad {

  lastDataRouting$: BehaviorSubject<any> = new BehaviorSubject(undefined);

  constructor(private router: Router,
    private auth: AuthService
  ) { }

  canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    throw new Error("Method not implemented.");
  }
  canLoad(route: Route): boolean {
    throw new Error("Method not implemented.");
  }

  async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
    var lengthRoles = route.data.roles.length;
    var count = 0;
    var array = this.auth.getRoles().split(",");

    if (lengthRoles == 4) {
      //1 Route:
      if (!array.includes(route.data.roles[0])) {
        this.router.navigate(['/error/403']);
        return false
      } else {
        for (let index = 1; index < lengthRoles; index++) {
          const element = route.data.roles[index];
          if (array.includes(element)) {
            count++
          }
        }
        if (count > 1) {
          return true;
        } else {
          this.router.navigate(['/error/403']);
          return false
        }
      }
    } else if (lengthRoles == 5 || lengthRoles == 10 || lengthRoles == 14 || lengthRoles == 22) {
      //2 Route + 3 Special Case:
      if (!array.includes(route.data.roles[0]) && !array.includes(route.data.roles[1])) {
        this.router.navigate(['/error/403']);
        return false
      } else {
        for (let index = 2; index < lengthRoles; index++) {
          const element = route.data.roles[index];
          if (array.includes(element)) {
            count++
          }
        }
        if (count > 1) {
          return true;
        } else {
          this.router.navigate(['/error/403']);
          return false
        }
      }
    } else if (lengthRoles == 6) {
      //3 Route:
      if (!array.includes(route.data.roles[0]) && !array.includes(route.data.roles[1]) && !array.includes(route.data.roles[2])) {
        this.router.navigate(['/error/403']);
        return false
      }
      for (let index = 3; index < lengthRoles; index++) {
        const element = route.data.roles[index];
        if (array.includes(element)) {
          count++
        }
      }
      if (count > 1) {
        return true;
      } else {
        this.router.navigate(['/error/403']);
        return false
      }
    } else {
      //... Route:
      this.router.navigate(['/error/403']);
      return false
    }
  }

    // Code cũ:
    // var count = 0;
    // for (let index = 0; index < route.data.roles.length; index++) {
    //   const element = route.data.roles[index];
    //   var array = this.auth.getRoles().split(",");
    //   if (array.includes(element)) {
    //     count++
    //   }
    // }
    // if (count > 0) {
    //   return true;
    // } else {
    //   this.router.navigate(['/error/403']);
    //   return false
    // }

  // async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
  //   const roles = route.data.roles;
  //   const userRoles = this.auth.getRoles().split(",");
    
  //   const requiredRoles = roles.slice(0, -1);
  //   const minCount = roles[roles.length - 1];
    
  //   const hasRequiredRoles = requiredRoles.every((role: any) => userRoles.includes(role));
  //   const matchingRoles = userRoles.filter((role: any) => roles.includes(role));
    
  //   if (hasRequiredRoles && matchingRoles.length >= minCount) {
  //     this.router.navigate(['/error/403']);
  //     return false;
  //   } else {
  //     return true;
  //   }
  // }
}
