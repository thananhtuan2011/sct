import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditGroupUserModalComponent } from './edit-group-user-modal.component';

describe('EditGroupUserModalComponent', () => {
  let component: EditGroupUserModalComponent;
  let fixture: ComponentFixture<EditGroupUserModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditGroupUserModalComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditGroupUserModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
