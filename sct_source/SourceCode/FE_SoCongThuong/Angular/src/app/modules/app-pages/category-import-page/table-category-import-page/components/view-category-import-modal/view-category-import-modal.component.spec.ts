import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewCategoryImportModalComponent } from './view-category-import-modal.component';

describe('ViewCategoryImportModalComponent', () => {
  let component: ViewCategoryImportModalComponent;
  let fixture: ComponentFixture<ViewCategoryImportModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewCategoryImportModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewCategoryImportModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
