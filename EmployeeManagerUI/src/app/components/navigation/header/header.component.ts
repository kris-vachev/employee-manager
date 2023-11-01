import { Component } from '@angular/core';
import { User } from 'src/app/interfaces/user';
import { AccountService } from 'src/app/services/account.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent{

  constructor(
    private _accountService: AccountService,
    private _userService: UserService
  ){ }

  logout(): void{
    if (confirm("Do you want to logout?")) {
      this._accountService.logout();
    }
  }

  get isLoggedIn(): boolean{
    return this._accountService.isLoggedIn();
  }

  get user(): User | null{
    return this._userService.getUser();
  }
}
