import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-nav-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './nav-header.html',
  styleUrl: './nav-header.css'
})
export class NavHeader implements OnInit {
  constructor(public authService: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  get role(): string | null { return this.authService.getRole(); }
  get username(): string | null { return this.authService.getUsername(); }

  // single-letter avatar shown in the header
  get initials(): string {
    const name = this.username;
    if (!name) return '?';
    return name.trim().charAt(0).toUpperCase();
  }

  // re-renders the header on every route change
  ngOnInit(): void {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.cdr.detectChanges();
      }
    });
  }

  // closes the session and sends the user back to the login page
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}