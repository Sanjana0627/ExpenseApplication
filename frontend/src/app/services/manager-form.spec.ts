import { TestBed } from '@angular/core/testing';

import { ManagerForm } from './manager-form';

describe('ManagerForm', () => {
  let service: ManagerForm;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManagerForm);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
