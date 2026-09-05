import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {Router} from '@angular/router';
import { ExpenseFormService } from '../../services/expense-form'; 
import { AppCurrencyPipe } from '../../app-currency';
@Component({
  selector: 'app-employee-forms-list',
  standalone:true,
  imports: [CommonModule,FormsModule,AppCurrencyPipe],
  templateUrl: './employee-forms-list.html',
  styleUrl: './employee-forms-list.css',
})
export class EmployeeFormsList implements OnInit{
  forms: any[]=[];
  statusFilter='';
  fromDate='';
  toDate='';
  errorMessage='';
  editableStatuses=['PendingApproval','Rejected'];

  constructor(
    private expenseFormService: ExpenseFormService,
    private router:Router,
    private cdr: ChangeDetectorRef
  ){}

  ngOnInit(): void {
    this.loadForms();
  }
  // fetches this employee's own expense forms, applying filters if set
  loadForms():void{
    this.errorMessage='';
    this.expenseFormService.getMyExpenseForms({
      status:this.statusFilter||undefined,
      fromDate:this.fromDate||undefined,
      toDate:this.toDate||undefined
    }).subscribe({
      next: (forms)=>{
        this.forms=forms;
        this.cdr.detectChanges();
      },
      error: (err)=>{
        this.errorMessage=err.error?.message || 'Could not load expense forms.';
        this.cdr.detectChanges();
      }
    });
  }
  
  applyFilters():void{
    this.loadForms();
  }
  
  clearFilters():void{
    this.statusFilter='';
    this.fromDate='';
    this.toDate='';
    this.loadForms();
  }
  
  isEditable(status:string):boolean{
    return this.editableStatuses.includes(status);
  }
  
  goToNewForm():void{
    this.router.navigate(['/dashboard/new'])
  }
  
  goToEdit(formId:number):void{
    this.router.navigate(['/dashboard/edit',formId]);
  }
  
  goToView(formId:number):void{
    this.router.navigate(['/dashboard/view',formId]);
  }
}
