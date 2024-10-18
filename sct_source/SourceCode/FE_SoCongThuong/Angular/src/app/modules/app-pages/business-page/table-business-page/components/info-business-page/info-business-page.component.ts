import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { PageInfoService, PageLink } from 'src/app/_metronic/layout';
import { BusinessPageService } from '../../../_services/business-page.service';


@Component({
  selector: 'app-info-business-page',
  templateUrl: './info-business-page.component.html',
  styleUrls: ['./info-business-page.component.scss'],

})
export class InfoBusinessPageComponent implements OnInit, OnDestroy {
  isLoading$: any;
  infoData: any;

  private subscriptions: Subscription[] = [];
  public industryData: { id: string, code: string, name: string }[] = [];
  public dataSource: { code: string, name: string }[] = [];
  public id: any;

  // links: Array<PageLink> = [{
  //   title: 'Main title',
  //   path: '/',
  //   isActive: false,
  // }];


  constructor(
    private businessService: BusinessPageService,
    private route: ActivatedRoute,
    private cd: ChangeDetectorRef,
    private router: Router,
    // private pageInfo: PageInfoService
  ) { 
    // pageInfo.setTitle('Page title');
    // pageInfo.setBreadcrumbs(this.links);
    // console.log(pageInfo.breadcrumbs)
  }

  ngOnInit(): void {
    this.isLoading$ = this.businessService.isLoading$;
    this.route.params.subscribe((params) => {
      this.id = params.id
      this.loadData(this.id); 
    });
    
    // 
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  loadData(id: any) {
    this.businessService.viewinfo(id).subscribe((res: any) => {
      this.infoData = res.data.databusiness
      this.cd.detectChanges();
      this.infoData.loaiHinhDoanhNghiep = res.data.datatypeofbusiness
      this.infoData.loaiNganhNghe = res.data.datatypeofprofession

      res.data.dataindustry.forEach((element: any) => {
        const data = {
          code: element.industryCode,
          name: element.industryName
        }
        this.dataSource.push(data)
      });
      this.dataSource = this.dataSource.sort((i1, i2) => {
        if (i1.code > i2.code) {
          return 1;
        }
        if (i1.code < i2.code) {
          return -1;
        }
        return 0;
      })
      this.cd.detectChanges();
    })
  }

  back(){
    this.router.navigate(['/business/list']);
  }
}
