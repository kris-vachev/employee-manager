import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {
    constructor(
        private _accountService: AccountService, 
        private _userService: UserService,
        private router: Router
    ) {}

    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
      const isLoggedIn = this._accountService.isLoggedIn();
      // get roles if a logged user
      if (isLoggedIn) {
        const requiredRoles = next.data['roles'] as string[];
        const userRole = this._userService.getUser()?.role.roleName;
        
        // verify required roles
        if (requiredRoles && requiredRoles.length > 0 && userRole && !requiredRoles.includes(userRole)) {
          this.router.navigate(['/unauthorized']);
          return false;
        }
  
        return true;
      } else {
        // navigation to login screen
        this.router.navigate(['/login']);
        return false;
      }
    }
}
