import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ManagerFormService } from '../../services/manager-form';
import { AppCurrencyPipe } from '../../app-currency';
@Component({
  selector: 'app-manager-review',
  standalone: true,
  imports: [CommonModule, FormsModule,AppCurrencyPipe],
  templateUrl: './manager-review.html',
  styleUrl: './manager-review.css',
})
export class ManagerReview implements OnInit {
  formId!: number;
  form: any = null;
  showRejectInput = false;
  rejectReason = '';
  successMessage = '';
  errorMessage = '';

  // wires up the services this component needs
  constructor(
    private managerFormService: ManagerFormService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  // reads the form id from the route and loads that form
  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const idParam = params.get('id');
      if (idParam) {
        this.formId = Number(idParam);
        this.loadForm();
      }
    });
  }

  // fetches the single expense form this manager is about to review
  loadForm(): void {
    this.errorMessage = '';
    this.managerFormService.getFormForReview(this.formId).subscribe({
      next: (form) => {
        this.form = form;
        this.cdr.detectChanges();
      },
      error: (err) => {
        const message = err.error?.message;
        this.errorMessage = message ?? 'Could not load this expense form.';
        this.cdr.detectChanges();
      }
    });
  }

  // approves the form, then bounces back to the list 
  approve(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.managerFormService.approveForm(this.formId).subscribe({
      next: () => {
        this.successMessage = 'Form approved successfully.';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/manager']), 1500);
      },
      error: (err) => {
        const message = err.error?.message;
        this.errorMessage = message ?? 'Could not approve this form.';
        this.cdr.detectChanges();
      }
    });
  }

  // shows the box where the manager types their rejection reason
  openRejectInput(): void {
    this.showRejectInput = true;
    this.cdr.detectChanges();
  }

  closeRejectInput(): void {
    this.showRejectInput = false;
    this.rejectReason = '';
    this.cdr.detectChanges();
  }

  // rejects the form with the typed reason, then bounces back to the list
  confirmReject(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.managerFormService.rejectForm(this.formId, this.rejectReason).subscribe({
      next: () => {
        this.successMessage = 'Form rejected.';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/manager']), 1200);
      },
      error: (err) => {
        const message = err.error?.message;
        this.errorMessage = message ?? 'Could not reject this form.';
        this.cdr.detectChanges();
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/manager']);
  }
}