import {Injectable} from '@angular/core';
import {HttpClient,HttpHeaders,HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../environments/environment';
import {AuthService} from './auth';
@Injectable({
    providedIn:'root'
})
export class AccountantFormService{
    private apiUrl=`${environment.apiUrl}/Accountant`;
    constructor(private http:HttpClient,private authService: AuthService){}
    // builds the auth header every request needs to send
    private getAuthHeader(){
        return {
            Authorization: `Bearer ${this.authService.getToken()}`
        };
    }
    
    getPendingPayments(filters?: {fromDate?:string;toDate?:string}): Observable<any[]>{
        let params=new HttpParams();
        if(filters?.fromDate) params=params.set('fromDate',filters.fromDate);
        if(filters?.toDate) params=params.set('toDate',filters.toDate);
        return this.http.get<any[]>(`${this.apiUrl}/expense-forms`,{
            headers:this.getAuthHeader(),
            params
        });
    }
    // gets the full detail of one form so the accountant can review it before paying
    getFormForReview(formId:number):Observable<any>{
        return this.http.get<any>(
            `${this.apiUrl}/expense-forms/${formId}`,
            {headers:this.getAuthHeader()}
        );
    }
    // marks a form as paid
    payForm(formId:number):Observable<{message:string}>{
        return this.http.put<{message:string}>(
            `${this.apiUrl}/expense-forms/${formId}/pay`,
            {},
            {headers:this.getAuthHeader()}
        );
    }
}