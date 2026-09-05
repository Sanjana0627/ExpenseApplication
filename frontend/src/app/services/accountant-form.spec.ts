import { TestBed } from '@angular/core/testing';

import { AccountantForm } from './accountant-form';

describe('AccountantForm', () => {
  let service: AccountantForm;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AccountantForm);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
