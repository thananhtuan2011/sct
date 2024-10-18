import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter, Subscription } from 'rxjs';
import { AuthService } from 'src/app/modules/auth';
import { PageInfoService } from '../../../core/page-info.service';

@Component({
  selector: 'app-sidebar-menu',
  templateUrl: './sidebar-menu.component.html',
  styleUrls: ['./sidebar-menu.component.scss']
})
export class SidebarMenuComponent implements OnInit, OnDestroy {
  constructor(
    private _AuthService: AuthService,
    private detectchanges: ChangeDetectorRef,
    private pageInfo: PageInfoService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }
  _menu: any = [];
  currentUrl: string;
  subscriptions: Subscription[] = [];

  ngOnInit(): void {
    // router subscription
    this.currentUrl = this.router.url.split(/[?#]/)[0];
    const routerSubscr = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.currentUrl = event.url;
      this.cdr.detectChanges();
    });
    this.subscriptions.push(routerSubscr);

    this._AuthService.getMenu().subscribe(res => {
      this._menu = res.data;
      this.findRoot(this._menu, {})
      this.detectchanges.detectChanges();
      setTimeout(() => {
        this.pageInfo.calculateTitle();
        this.pageInfo.calculateBreadcrumbs();
      }, 10);
    })
  }

  findRoot(node: any, parent: any) {
    node.forEach((item: any) => {
      if (item['child']) {
        if (item['child'].length > 0) {
          item.open = this.checkOpen(item['child'], item)
          this.findRoot(item['child'], item)
        }
      }
    });
  }

  checkOpen(node: any, parent: any) {
    for (let index = 0; index < node.length; index++) {
      const item = node[index];
      if (this.isMenuItemActive(item.href)) {
        return true
      }
      if (item['child']) {
        if (item['child'].length > 0) {
          if (this.checkOpen(item['child'], item)) {
            return true
          }
        }
      }
    }
  }

  isMenuItemActive(path: any) {
    if (!this.currentUrl || !path) {
      return false;
    }

    if (this.currentUrl === path) {
      return true;
    }

    if (this.currentUrl.indexOf(path) > -1) {
      return true;
    }

    return false;
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

}
