import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableCategoryImportPageComponent } from './table-category-import-page.component';

describe('TableCategoryImportPageComponent', () => {
  let component: TableCategoryImportPageComponent;
  let fixture: ComponentFixture<TableCategoryImportPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableCategoryImportPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableCategoryImportPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
