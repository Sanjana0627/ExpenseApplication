import { Component,OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminFormService } from '../../services/admin-form';
import {Router} from '@angular/router';
import { AppCurrencyPipe } from '../../app-currency';
@Component({
  selector: 'app-admin-dashboard',
  standalone:true,
  imports: [CommonModule, FormsModule,AppCurrencyPipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css',
})
export class AdminDashboard implements OnInit {
  forms: any[]=[];
  statusFilter='';
  fromDate='';
  toDate='';
  errorMessage='';
  selectedForm: any=null;
  constructor(private adminFormService: AdminFormService, private router: Router, private cdr: ChangeDetectorRef) {}
  // runs once the page loads - kicks off the first fetch of all forms
  ngOnInit(): void {
    this.loadForms();
  }
  // fetches every expense form
  loadForms(): void{
    this.errorMessage='';
    this.adminFormService.getAllForms({
      status:this.statusFilter||undefined,
      fromDate:this.fromDate||undefined,
      toDate:this.toDate||undefined
    }).subscribe({
      next: (forms)=>{ this.forms=forms; this.cdr.detectChanges(); },
      error: (err)=>{ this.errorMessage=err.error?.message || 'Could not load expense forms.'; this.cdr.detectChanges(); }
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
  goToReports(): void {
    this.router.navigate(['/admin/reports']);
  }
  viewForm(form:any):void{
    this.selectedForm=form;
  }
  closeView():void{
    this.selectedForm=null;
  }
}
