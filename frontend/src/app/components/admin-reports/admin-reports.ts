import { Component, OnInit, ViewChild, ElementRef, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Chart, registerables } from 'chart.js';
import { AdminFormService } from '../../services/admin-form';

Chart.register(...registerables);

@Component({
  selector: 'app-admin-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-reports.html',
  styleUrl: './admin-reports.css'
})
export class AdminReports implements OnInit, AfterViewInit {
  @ViewChild('statusChart') statusChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('monthlyChart') monthlyChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('rejectionChart') rejectionChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('categoryChart') categoryChartRef!: ElementRef<HTMLCanvasElement>;

  turnaroundHours: number | null = null;
  turnaroundSampleSize = 0;
  errorMessage = '';

  constructor(private adminFormService: AdminFormService, private router: Router, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.loadStatusBreakdown();
    this.loadMonthlySpend();
    this.loadRejectionRate();
    this.loadAverageTurnaround();
    this.loadSpendByCategory();
  }

  // draws the pie chart of how many forms are in each status
  loadStatusBreakdown(): void {
    this.adminFormService.getStatusBreakdown().subscribe({
      next: (data) => {
        new Chart(this.statusChartRef.nativeElement, {
          type: 'pie',
          data: {
            labels: data.map(d => d.status),
            datasets: [{
              data: data.map(d => d.count),
              backgroundColor: ['#fbbf24', '#60a5fa', '#f87171', '#a78bfa', '#34d399']
            }]
          }
        });
      },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load status report.'; this.cdr.detectChanges(); }
    });
  }

  // draws the line chart of forms submitted per month
  loadMonthlySpend(): void {
    this.adminFormService.getMonthlyFormCount().subscribe({
      next: (data) => {
        new Chart(this.monthlyChartRef.nativeElement, {
          type: 'line',
          data: {
            labels: data.map(d => d.month),
            datasets: [{
              label: 'Forms Submitted',
              data: data.map(d => d.formCount),
              borderColor: '#2563eb',
              tension: 0.3
            }]
          }
        });
      },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load monthly report.'; this.cdr.detectChanges(); }
    });
  }

  // draws the bar chart comparing each manager's rejection rate
  loadRejectionRate(): void {
    this.adminFormService.getRejectionRateByManager().subscribe({
      next: (data) => {
        new Chart(this.rejectionChartRef.nativeElement, {
          type: 'bar',
          data: {
            labels: data.map(d => d.managerName),
            datasets: [{
              label: 'Rejection Rate (%)',
              data: data.map(d => d.rejectionRatePercent),
              backgroundColor: '#f87171'
            }]
          },
          options: { scales: { y: { beginAtZero: true, max: 100 } } }
        });
      },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load rejection rate.'; this.cdr.detectChanges(); }
    });
  }

  // fetches the average approval turnaround time as a plain stat, no chart
  loadAverageTurnaround(): void {
    this.adminFormService.getAverageTurnaround().subscribe({
      next: (data) => {
        this.turnaroundHours = data.averageTurnaroundHours;
        this.turnaroundSampleSize = data.sampleSize;
        this.cdr.detectChanges();
      },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load turnaround time.'; this.cdr.detectChanges(); }
    });
  }

  // draws the doughnut chart of expense counts by category
  loadSpendByCategory(): void {
    this.adminFormService.getExpenseCountByCategory().subscribe({
      next: (data) => {
        new Chart(this.categoryChartRef.nativeElement, {
          type: 'doughnut',
          data: {
            labels: data.map(d => d.category),
            datasets: [{
              data: data.map(d => d.count),
              backgroundColor: ['#60a5fa', '#fbbf24', '#34d399', '#a78bfa', '#f87171', '#94a3b8']
            }]
          }
        });
      },
      error: (err) => { this.errorMessage = err.error?.message || 'Could not load category report.'; this.cdr.detectChanges(); }
    });
  }
  goBack(): void {
    this.router.navigate(['/admin']);
  }
}