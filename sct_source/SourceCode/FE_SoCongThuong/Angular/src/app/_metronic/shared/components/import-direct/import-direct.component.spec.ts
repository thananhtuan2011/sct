import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportDirectComponent } from './import-direct.component';

describe('ImportDirectComponent', () => {
  let component: ImportDirectComponent;
  let fixture: ComponentFixture<ImportDirectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImportDirectComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ImportDirectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
