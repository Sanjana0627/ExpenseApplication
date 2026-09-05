import { Component ,OnInit, ChangeDetectorRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {AccountantFormService} from '../../services/accountant-form';
import { AppCurrencyPipe } from '../../app-currency';
import { Router } from '@angular/router';
@Component({
  selector: 'app-accountant-dashboard',
  standalone:true,
  imports: [CommonModule, FormsModule,AppCurrencyPipe],
  templateUrl: './accountant-dashboard.html',
  styleUrl: './accountant-dashboard.css',
})
export class AccountantDashboard implements OnInit{
  forms: any[]=[];
  fromDate='';
  toDate='';
  errorMessage='';
  constructor(private accountantFormService: AccountantFormService,private router: Router,private cdr: ChangeDetectorRef){}
  
  ngOnInit(): void {
    this.loadForms();
  }
  // fetches the accountant's pending-payment forms, applying date filters if set
  loadForms():void{
    this.errorMessage='';
    this.accountantFormService.getPendingPayments({
      fromDate:this.fromDate||undefined,
      toDate:this.toDate||undefined
    }).subscribe({
      next: (forms)=>{ this.forms=forms; this.cdr.detectChanges(); },
      error: (err)=>{ this.errorMessage=err.error?.message || 'Could not load pending payments.'; this.cdr.detectChanges(); }
    });
  }
  applyFilters():void{
    this.loadForms();
  }
  clearFilters():void{
    this.fromDate='';
    this.toDate='';
    this.loadForms();
  }
  goToReview(formId: number): void {
    this.router.navigate(['/accountant/review', formId]);
  }
}
