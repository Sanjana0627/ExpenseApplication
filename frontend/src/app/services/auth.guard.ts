import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth';

/** Blocks any route unless there's a valid session - sends you to login instead
 *  of leaving you on a half-rendered page with no header. */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};

/** Same as authGuard, but also checks the token's role matches one of the
 *  roles allowed on this route. Wrong role gets bounced to their own home
 *  page instead of a page full of 403s. */
export function roleGuard(allowedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isLoggedIn()) {
      router.navigate(['/login']);
      return false;
    }

    const role = authService.getRole();
    if (role && allowedRoles.includes(role)) {
      return true;
    }

    router.navigate([homePathForRole(role)]);
    return false;
  };
}

// picks each role's own home page, used to redirect a logged-in user away from a route they can't access
function homePathForRole(role: string | null): string {
  switch (role) {
    case 'Employee': return '/dashboard';
    case 'Manager': return '/manager';
    case 'Accountant': return '/accountant';
    case 'Admin': return '/admin';
    default: return '/login';
  }
}
