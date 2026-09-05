import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountantFormService } from '../../services/accountant-form';
import { AppCurrencyPipe } from '../../app-currency';
@Component({
  selector: 'app-accountant-review',
  standalone: true,
  imports: [CommonModule,AppCurrencyPipe],
  templateUrl: './accountant-review.html',
  styleUrl: './accountant-review.css',
})
export class AccountantReview implements OnInit {
  formId!: number;
  form: any = null;
  successMessage = '';
  errorMessage = '';

  // wires up the services this component needs
  constructor(
    private accountantFormService: AccountantFormService,
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

  loadForm(): void {
    this.errorMessage = '';
    this.accountantFormService.getFormForReview(this.formId).subscribe({
      next: (form) => {
        this.form = form;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Could not load this expense form.';
        this.cdr.detectChanges();
      }
    });
  }

  // marks the form as paid, then bounces back to the list after a short delay
  pay(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.accountantFormService.payForm(this.formId).subscribe({
      next: () => {
        this.successMessage = 'Form marked as paid successfully.';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/accountant']), 1200);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Could not mark this form as paid.';
        this.cdr.detectChanges();
      }
    });
  }
  goBack(): void {
    this.router.navigate(['/accountant']);
  }
}