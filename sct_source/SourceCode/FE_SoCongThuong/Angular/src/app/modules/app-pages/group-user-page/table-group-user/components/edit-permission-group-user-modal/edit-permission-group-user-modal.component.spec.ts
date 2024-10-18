import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPermissionGroupUserModalComponent } from './edit-permission-group-user-modal.component';

describe('EditPermissionGroupUserModalComponent', () => {
  let component: EditPermissionGroupUserModalComponent;
  let fixture: ComponentFixture<EditPermissionGroupUserModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditPermissionGroupUserModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditPermissionGroupUserModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
