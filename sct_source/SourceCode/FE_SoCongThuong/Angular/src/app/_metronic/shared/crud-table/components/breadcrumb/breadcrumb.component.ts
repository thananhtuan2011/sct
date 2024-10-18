import { PageInfoService, PageLink } from './../../../../layout/core/page-info.service';
import { Component, OnInit, Input, OnChanges, ElementRef, Output, EventEmitter } from '@angular/core';
import { SortDirection } from '../../models/sort.model';
import { BehaviorSubject, Observable } from 'rxjs';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html'
})
export class BreadcrumbComponent implements OnInit {
  bc$: Observable<Array<PageLink>>;
  title$: Observable<string>;
  @Input() BreadcrumbType: number = 0;
  public breadcrumbsDefault: BehaviorSubject<Array<PageLink>> = new BehaviorSubject<Array<PageLink>>([]);
  constructor( private pageInfo: PageInfoService ) {
    setTimeout(() => {
      this.pageInfo.calculateTitle();
      this.pageInfo.calculateBreadcrumbs();
    }, 10);
  }

  ngOnInit(): void {
    this.title$ = this.pageInfo.title.asObservable();
    this.bc$ = this.pageInfo.breadcrumbs.asObservable();
    // if(this.BreadcrumbType == 0){
    //   this.bc$ = this.pageInfo.breadcrumbs.asObservable();
    // }else{
    //   this.bc$ = this.breadcrumbsDefault.asObservable();
    // }
  }
}
