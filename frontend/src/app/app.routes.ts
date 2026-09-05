import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { EmployeeDashboard } from './components/employee-dashboard/employee-dashboard';
import { EmployeeFormsList } from './components/employee-forms-list/employee-forms-list';
import { ManagerDashboard } from './components/manager-dashboard/manager-dashboard';
import { AccountantDashboard } from './components/accountant-dashboard/accountant-dashboard';
import { AdminDashboard } from './components/admin-dashboard/admin-dashboard';
import { ManagerReview } from './components/manager-review/manager-review';
import { AccountantReview } from './components/accountant-review/accountant-review';
import { AdminReports } from './components/admin-reports/admin-reports';
import { roleGuard } from './services/auth.guard';

// every page in the app, each guarded so only the right role can open it
export const routes: Routes = [
  { path: '',redirectTo:'login',pathMatch:'full'},
  { path: 'login', component: Login },
  { path: 'dashboard', component: EmployeeFormsList, canActivate: [roleGuard(['Employee'])] },
  { path: 'dashboard/new', component: EmployeeDashboard, canActivate: [roleGuard(['Employee'])] },
  { path: 'dashboard/edit/:id',component:EmployeeDashboard, canActivate: [roleGuard(['Employee'])] },
  { path: 'dashboard/view/:id', component: EmployeeDashboard, data: { viewOnly: true }, canActivate: [roleGuard(['Employee'])] },
  { path: 'manager', component: ManagerDashboard, canActivate: [roleGuard(['Manager'])] },
  { path: 'accountant', component: AccountantDashboard, canActivate: [roleGuard(['Accountant'])] },
  { path: 'admin',component:AdminDashboard, canActivate: [roleGuard(['Admin'])] },
  { path: 'manager/review/:id', component: ManagerReview, canActivate: [roleGuard(['Manager'])] },
  { path: 'accountant/review/:id', component: AccountantReview, canActivate: [roleGuard(['Accountant'])] },
  { path: 'admin/reports', component: AdminReports, canActivate: [roleGuard(['Admin'])] },
  { path: '**', redirectTo: 'login' },
];
