import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth';

@Injectable({
  providedIn: 'root'
})
export class ManagerFormService {
  private apiUrl = `${environment.apiUrl}/Manager`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  // builds the auth header every request needs to send
  private getAuthHeader() {
    return {
      Authorization: `Bearer ${this.authService.getToken()}`
    };
  }

  // gets forms waiting on this manager's approval, with optional date filters
  getPendingApprovals(filters?: { fromDate?: string; toDate?: string }): Observable<any[]> {
    let params = new HttpParams();
    if (filters?.fromDate) params = params.set('fromDate', filters.fromDate);
    if (filters?.toDate) params = params.set('toDate', filters.toDate);

    return this.http.get<any[]>(`${this.apiUrl}/expense-forms`, {
      headers: this.getAuthHeader(),
      params
    });
  }
  // gets the full detail of one form so the manager can review it
  getFormForReview(formId:number):Observable<any>{
    return this.http.get<any>(
        `${this.apiUrl}/expense-forms/${formId}`,
        {headers:this.getAuthHeader()}
    );
  }
  // approves a form, moving it on to the accountant
  approveForm(formId:number):Observable<{message:string}>{
    return this.http.put<{message:string}>(
        `${this.apiUrl}/expense-forms/${formId}/approve`,
        {},
        { headers: this.getAuthHeader() }
    );
  }
  // rejects a form with a required reason, sending it back to the employee
  rejectForm(formId:number,reason:string):Observable<{message:string}>{
    return this.http.put<{message:string}>(
        `${this.apiUrl}/expense-forms/${formId}/reject`,
        {reason},
        { headers: this.getAuthHeader() }
    );
  }
}