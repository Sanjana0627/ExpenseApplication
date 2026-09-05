import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import {FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';
import {ExpenseFormService,ExpenseLineItem} from '../../services/expense-form';
import { ActivatedRoute, Router } from '@angular/router';
import { AppCurrencyPipe } from '../../app-currency';
function emptyLine(): ExpenseLineItem {
  return { expenseDate: '', category: '', purpose: '', vendorName: '', paymentMethod: '', amount: 0 };
}

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule,AppCurrencyPipe],
  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css',
})
export class EmployeeDashboard implements OnInit{
  currencyId=1;
  expenses: ExpenseLineItem[] = [emptyLine()];
  formId: number | null = null;
  draftFormId: number | null = null;
  rejectionReason: string | null = null;
  successMessage='';
  errorMessage='';
  viewOnly=false;
  submitted=false;
  today = new Date().toISOString().split('T')[0];

  constructor(
    private expenseFormService: ExpenseFormService,
    private route:ActivatedRoute,
    private router:Router,
    private cdr: ChangeDetectorRef
  ) {}

  // figures out whether this is a new form, an edit, or a view-only page from the route,
  // then loads the existing form or starts a blank one
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.viewOnly = this.route.snapshot.data['viewOnly'] === true;

      const idParam = params.get('id');
      this.successMessage = '';
      this.errorMessage = '';
      this.submitted = false;
      this.isEditable = true;

      if (idParam) {
        this.formId = Number(idParam);
        this.loadExistingForm(this.formId);
      } else {
        this.formId = null;
        this.currencyId = 1;
        this.expenses = [emptyLine()];
      }
    });
  }
  isEditable = true;
  // loads a form that already exists and fills the fields in
  loadExistingForm(formId: number): void{
    this.expenseFormService.getExpenseForm(formId).subscribe({
      next: (form)=>{
        this.currencyId=form.currencyId;
        this.expenses=form.expenses.map((e: any)=>({
          expenseDate: e.expenseDate.split('T')[0],
          category: e.category,
          purpose: e.purpose,
          vendorName: e.vendorName ?? '',
          paymentMethod: e.paymentMethod ?? '',
          amount: e.amount
        }));
        this.rejectionReason = form.status === 'Rejected' ? (form.rejectionReason ?? null) : null;
        if(form.status !== 'PendingApproval' && form.status !== 'Rejected'){
          this.isEditable=false;
          if(!this.viewOnly){
            this.errorMessage=`This form cannot be edited because its status is '${form.status}'.`;
          }
        }
        this.cdr.detectChanges();
      },
      error: (err)=>{
        this.errorMessage=err.error?.message || `Could not load this expense form.`;
        this.cdr.detectChanges();
      }
    });
  }

  get totalAmount(): number {
    return this.expenses.reduce((sum,e)=> sum+(Number(e.amount)||0),0);
  } 

  addExpenseLine():void{
    this.expenses.push(emptyLine());
  }

  removeExpenseLine(index:number):void{
    if(this.expenses.length>1){
      this.expenses.splice(index,1);
    }
  }

  // returns true if every line has what it needs and no date is in the future.
  // this only checks client-side stuff - the API still re-validates everything.
  private formIsValid(): boolean {
    if (this.expenses.length === 0) return false;
    return this.expenses.every(e =>
      !!e.expenseDate &&
      e.expenseDate <= this.today &&
      !!e.category &&
      !!e.purpose.trim() &&
      !!e.vendorName.trim() &&
      !!e.paymentMethod &&
      Number(e.amount) > 0
    );
  }

  // validates the form, then creates a new one or updates the existing one
  onSubmit(): void{
    this.successMessage='';
    this.errorMessage='';
    this.submitted=true;

    if (!this.formIsValid()) {
      this.errorMessage = 'Please fill in the highlighted fields before submitting.';
      this.cdr.detectChanges();
      return;
    }

    const request={
      currencyId:this.currencyId,
      expenses:this.expenses
    };
    const request$=this.formId
      ? this.expenseFormService.updateExpenseForm(this.formId,request)
      : this.expenseFormService.createExpenseForm(request);

    request$.subscribe({
      next:(response:any)=>{
        this.successMessage=this.formId
          ? `Expense form updated successfully.`
          : `Expense form submitted successfully.Form ID: ${response.formId}`;
          this.rejectionReason = null;
          if(!this.formId){
            this.expenses=[emptyLine()];
            this.submitted=false;
          }
        this.cdr.detectChanges();
      },
      error:(err)=>{
        this.errorMessage=err.error?.message || `Failed to submit expense form`;
        this.cdr.detectChanges();
      }
    });
  }

  goBackToList(): void {
    this.router.navigate(['/dashboard']);
  }
}
