import { Injectable } from '@angular/core';
import {HttpClient,HttpHeaders,HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../environments/environment';
import {AuthService} from './auth';
@Injectable({
    providedIn:'root'
})
export class AdminFormService{
    private apiUrl=`${environment.apiUrl}/Admin`;
    constructor(private http:HttpClient,private authService: AuthService){}
    // builds the auth header every request needs to send
    private getAuthHeader(){
        return {
            Authorization: `Bearer ${this.authService.getToken()}`
        };
    }
    // gets every expense form company-wide, with optional status/date filters
    getAllForms(filters?: {status?:string;fromDate?:string;toDate?:string}): Observable<any[]>{
        let params=new HttpParams();
        if(filters?.status) params=params.set('status',filters.status);
        if(filters?.fromDate) params=params.set('fromDate',filters.fromDate);
        if(filters?.toDate) params=params.set('toDate',filters.toDate);
        return this.http.get<any[]>(`${this.apiUrl}/expense-forms`,{
            headers:this.getAuthHeader(),
            params
        });
    }
    // how many forms are in each status - feeds the pie chart
    getStatusBreakdown(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/reports/status-breakdown`, { headers: this.getAuthHeader() });
    }

    // how many forms were submitted each month - feeds the line chart
    getMonthlyFormCount(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/reports/monthly-form-count`, { headers: this.getAuthHeader() });
    }

    // each manager's rejection rate - feeds the bar chart
    getRejectionRateByManager(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/reports/rejection-rate-by-manager`, { headers: this.getAuthHeader() });
    }

    // average time between submission and a decision, plus how many forms that's based on
    getAverageTurnaround(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/reports/average-turnaround`, { headers: this.getAuthHeader() });
    }

    // how many expenses fall into each category - feeds the doughnut chart
    getExpenseCountByCategory(): Observable<any[]> {
        return this.http.get<any[]>(`${this.apiUrl}/reports/expense-count-by-category`, { headers: this.getAuthHeader() });
    }
}                                                                                                                                                      