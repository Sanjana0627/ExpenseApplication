import { Component, ChangeDetectorRef } from '@angular/core';
import {FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';
import {Router} from '@angular/router';
import { AuthService } from '../../services/auth';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  username='';
  password='';
  errorMessage='';
  constructor(private authService: AuthService,private router:Router,private cdr: ChangeDetectorRef){}
  // logs the user in, then routes them to the home page for their role
  onSubmit(): void {
    this.errorMessage = '';
    console.log('Login onSubmit called');
    this.authService.login({ username: this.username, password: this.password }).subscribe({
      next: () => {
        const role = this.authService.getRole();

        switch (role) {
          case 'Employee':
            this.router.navigate(['/dashboard']);
            break;
          case 'Manager':
            this.router.navigate(['/manager']);
            break;
          case 'Accountant':
            this.router.navigate(['/accountant']);
            break;
          case 'Admin':
            this.router.navigate(['/admin']);
            break;
          default:
            this.router.navigate(['/login']);
        }
        console.log('Role from token:', role);
      },
      error: (err) => {
        this.errorMessage = 'Invalid username or password.';
        this.cdr.detectChanges();
      }
    });
    
  }
}
