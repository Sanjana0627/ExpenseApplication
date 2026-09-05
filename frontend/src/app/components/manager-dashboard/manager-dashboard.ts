import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManagerFormService } from '../../services/manager-form';
import { Router } from '@angular/router';
import { AppCurrencyPipe } from '../../app-currency';
@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule,AppCurrencyPipe],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css'
})
export class ManagerDashboard implements OnInit {
  forms: any[] = [];
  fromDate = '';
  toDate = '';
  errorMessage = '';
  successMessage = '';

  constructor(private managerFormService: ManagerFormService,private router:Router,private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.loadForms();
  }

  // fetches forms waiting on this manager's approval, applying date filters if set
  loadForms(): void {
    this.errorMessage = '';
    this.managerFormService.getPendingApprovals({
      fromDate: this.fromDate || undefined,
      toDate: this.toDate || undefined
    }).subscribe({
      next: (forms) => { this.forms = forms; this.cdr.detectChanges(); },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load pending approvals.'; this.cdr.detectChanges(); }
    });
  }

  
  applyFilters(): void {
    this.loadForms();
  }

  
  clearFilters(): void {
    this.fromDate = '';
    this.toDate = '';
    this.loadForms();
  }
  
  goToReview(formId: number): void {
    this.router.navigate(['/manager/review', formId]);
  }
}