import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard {
    constructor(
        private _accountService: AccountService, 
        private _userService: UserService,
        private router: Router
    ) {}

    // do not allow logged user to access login screen
    canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
      const isLoggedIn = this._accountService.isLoggedIn();
      if (isLoggedIn) {
        this.router.navigate(['/employees']);
        return false;
      }

      return true;
    }
}
