import { TestBed } from '@angular/core/testing';

import { ExpenseForm } from './expense-form';

describe('ExpenseForm', () => {
  let service: ExpenseForm;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExpenseForm);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
