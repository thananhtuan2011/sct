import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, Subscription } from 'rxjs';
import { PageInfoService, PageLink } from 'src/app/_metronic/layout';
import { CateIntegratedManagementPageService } from '../../../_services/cateintegratedmanagement-page.service';

@Component({
  selector: 'app-info-page',
  templateUrl: './info-page.component.html',
  styleUrls: ['./info-page.component.scss'],

})
export class InfoPageComponent implements OnInit, OnDestroy {
  isLoading$: any;
  public infoData: any;
  public disbursementData: any;
  public historyData: any;

  private subscriptions: Subscription[] = [];
  public id: any;

  constructor(
    private cateIntegratedManagementPageService: CateIntegratedManagementPageService,
    private route: ActivatedRoute,
    private cd: ChangeDetectorRef,
    public modal: NgbActiveModal,
  ) { 
  }

  ngOnInit(): void {
    this.loadData(this.id)
    this.isLoading$ = this.cateIntegratedManagementPageService.isLoading$;
    // this.route.params.subscribe((params) => {
    //   this.id = params.id
    //   this.loadData(this.id); 
    // });
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sb => sb.unsubscribe());
  }

  loadData(id: any) {
    this.cateIntegratedManagementPageService.viewinfo(id).subscribe((res: any) => {
      this.infoData = res.data
      // this.cd.detectChanges();
      // console.log(this.infoData)
    })
  }

  convert_date_string(string_date: string) {
    if (string_date) {
      var date = string_date.split("T")[0];
      var list = date.split("-"); //["year", "month", "day"]
      var result = list[2] + "/" + list[1] + "/" + list[0]
      return result
    }
    else {
      return ""
    }
  }

  f_currency(value: any, args?: any): any {
    let nbr = Number((value + '').replace(/,|-/g, ""));
    const result = (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
    return result
  }
}
