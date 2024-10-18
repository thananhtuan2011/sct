import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-import-direct',
  templateUrl: './import-direct.component.html',
  styleUrls: ['./import-direct.component.scss']
})
export class ImportDirectComponent implements OnInit {

  constructor(private router: Router,) { }

  ngOnInit(): void {
  }

  importFile(){
    var url = window.location.href.replace(window.location.origin + '/','');
    this.router.navigate(
      ['/import'],
      { queryParams: { url: url } }
    );
  }
}
