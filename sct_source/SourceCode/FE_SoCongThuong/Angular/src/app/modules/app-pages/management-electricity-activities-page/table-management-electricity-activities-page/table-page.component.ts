import { AfterViewInit, Component } from '@angular/core';

@Component({
  selector: 'app-table-page',
  templateUrl: './table-page.component.html'
})
export class TablePageComponent implements AfterViewInit {
  selectedIndex: number = 1;
  
  ngAfterViewInit(): void {
    this.selectedIndex = 0;
  }
}