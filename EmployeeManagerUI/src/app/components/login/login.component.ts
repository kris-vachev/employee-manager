import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Login } from 'src/app/interfaces/login';
import { AccountService } from 'src/app/services/account.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private _accountService: AccountService,
    private _userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    })
  }

  logIn(){
    if (this.loginForm.valid){
      const model: Login = {
        userName: this.loginForm.value.userName,
        password: this.loginForm.value.password
      };
      this._accountService.login(model).subscribe({
        next: (data) => {
          if (data.status){
            // set token
            localStorage.setItem('token', data.msg);
            // set user
            this._userService.setUser(data.value);
            // navigate
            this.router.navigate(['employees']);
          }
          else{
            this.showAlert(data.msg, 'Error');
          }
        },
        error: (e) => {
          this.showAlert('An error occurred!.', 'Error');}
      });
    }
  }

  showAlert(msg: string, title: string){
    this._snackBar.open(msg,title, {
      horizontalPosition: "end",
      verticalPosition: "top",
      duration: 3000
    });
  }
}
