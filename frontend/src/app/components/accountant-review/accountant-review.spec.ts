import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountantReview } from './accountant-review';

describe('AccountantReview', () => {
  let component: AccountantReview;
  let fixture: ComponentFixture<AccountantReview>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountantReview],
    }).compileComponents();

    fixture = TestBed.createComponent(AccountantReview);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
