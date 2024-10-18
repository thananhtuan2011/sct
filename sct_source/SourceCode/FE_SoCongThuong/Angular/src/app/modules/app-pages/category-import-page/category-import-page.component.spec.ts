import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoryImportPageComponent } from './category-import-page.component';

describe('CategoryImportPageComponent', () => {
  let component: CategoryImportPageComponent;
  let fixture: ComponentFixture<CategoryImportPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CategoryImportPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoryImportPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
