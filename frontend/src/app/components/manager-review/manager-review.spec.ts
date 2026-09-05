import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagerReview } from './manager-review';

describe('ManagerReview', () => {
  let component: ManagerReview;
  let fixture: ComponentFixture<ManagerReview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagerReview],
    }).compileComponents();

    fixture = TestBed.createComponent(ManagerReview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
