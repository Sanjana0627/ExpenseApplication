import { Injectable, Service } from '@angular/core';
import { HttpClient,HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth';
export interface ExpenseLineItem{
    expenseDate: string;
    category: string;
    purpose:string;
    vendorName:string;
    paymentMethod:string;
    amount:number;
}
export interface CreateExpenseFormRequest{
    currencyId: number;
    expenses: ExpenseLineItem[];
}
interface CreateExpenseFormResponse{
    message:string;
    formId:number;
}
@Injectable({
    providedIn:'root'
})
export class ExpenseFormService{
    private apiUrl=`${environment.apiUrl}/Employee`;
    constructor(private http:HttpClient,private authService: AuthService){}
    // builds the auth header every request needs to send
    private getAuthHeader():HttpHeaders{
        return new HttpHeaders({
            Authorization: `Bearer ${this.authService.getToken()}`
        });
    }
    // submits a brand new expense form
    createExpenseForm(request: CreateExpenseFormRequest): Observable<CreateExpenseFormResponse>{
        return this.http.post<CreateExpenseFormResponse>(
            `${this.apiUrl}/expense-forms`,
            request,
            {headers:this.getAuthHeader()}
        );
    }
    // gets one of this employee's own forms, for editing or viewing
    getExpenseForm(formId: number): Observable<any> {
        return this.http.get<any>(
            `${this.apiUrl}/expense-forms/${formId}`,
            { headers: this.getAuthHeader() }
        );
    }
    // saves changes to a form that's still Pending or was Rejected
    updateExpenseForm(formId: number, request: CreateExpenseFormRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
        `${this.apiUrl}/expense-forms/${formId}`,
        request,
        { headers: this.getAuthHeader() }
    );
    }
    // gets this employee's own expense forms, with optional status/date filters
    getMyExpenseForms(filters?:{status?:string;fromDate?:string;toDate?:string}): Observable<any[]>{
        let params=new HttpParams();
        if(filters?.status) params=params.set('status',filters.status);
        if(filters?.fromDate) params=params.set('fromDate',filters.fromDate);
        if(filters?.toDate) params=params.set('toDate',filters.toDate);
        return this.http.get<any[]>(
            `${this.apiUrl}/expense-forms`,
            {headers:this.getAuthHeader(),params}
        );
    }
}