import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable,tap} from 'rxjs';
import {environment} from '../../environments/environment';
interface LoginRequest{
    username: string;
    password: string;
}
interface LoginResponse{
    token: string;
}
@Injectable({
    providedIn:'root'
})
export class AuthService{
    private apiUrl = `${environment.apiUrl}/auth`;
    constructor(private http:HttpClient){}
    // logs the user in and saves the returned JWT so later requests can use it
    login(credentials: LoginRequest): Observable<LoginResponse>{
        return this.http.post<LoginResponse>(`${this.apiUrl}/login`, credentials).pipe(
            tap(response =>{
                localStorage.setItem('token',response.token);
            })
        );
    }

    logout(): void{
        localStorage.removeItem('token');
    }
    
    getToken(): string|null{
        return localStorage.getItem('token');
    }
    // true if there's a token saved at all
    isLoggedIn():boolean{
        return !!this.getToken();
    }
    // decodes the JWT's payload so the role/username claims can be read out of it
    private decodeToken(): any {
    const token = this.getToken();
    if (!token) return null;

    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const padded = base64.padEnd(base64.length + (4 - (base64.length % 4)) % 4, '=');
        const decoded = JSON.parse(atob(padded));
        return decoded;
    } catch {
        return null;
    }
}
    // the logged-in user's role, read out of the token's claims
    getRole(): string | null {
        const decoded = this.decodeToken();
        if (!decoded) return null;
        return decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded['role'] || null;  
    }
    // the logged-in user's display name, read out of the token's claims
    getUsername(): string | null {
        const decoded = this.decodeToken();
        if (!decoded) return null;
        return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded['unique_name'] || null;
    }
}