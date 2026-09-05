import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeFormsList } from './employee-forms-list';

describe('EmployeeFormsList', () => {
  let component: EmployeeFormsList;
  let fixture: ComponentFixture<EmployeeFormsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeFormsList],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeFormsList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
