import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableGroupUserComponent } from './table-group-user.component';

describe('TableGroupUserComponent', () => {
  let component: TableGroupUserComponent;
  let fixture: ComponentFixture<TableGroupUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TableGroupUserComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TableGroupUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
