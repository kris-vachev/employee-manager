import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http'
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, catchError, map, throwError } from 'rxjs';
import { ApiResponse } from '../interfaces/api-response';
import { Login } from '../interfaces/login';
import jwt_decode from 'jwt-decode';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private endpoint: string = environment.endpoint;
  private apiUrl: string = this.endpoint + 'api/account';

  constructor(
    private http:HttpClient, 
    private router: Router
  ) { }

  login(login: Login): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.apiUrl, login).pipe(
      catchError(this.handleError)
    );
  }

  logout(): void{
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  refreshToken(): Observable<string> {
    return this.http.post<ApiResponse>(`${this.apiUrl}/refresh-token`, {}).pipe(
      map((response: ApiResponse) => {
        if (response.status){
          return response.value;
        }
      }),
      catchError(this.handleError)
    );
  }

  isTokenExpired(): boolean {
    const token = localStorage.getItem('token');
    if (!token) {
      return true;
    }

    const tokenExpiration = this.getTokenExpiration(token);
    return tokenExpiration < new Date();
  }

  private getTokenExpiration(token: string): any {
    const decodedToken: any = jwt_decode(token);
    if (!decodedToken.exp) {
      return null;
    }

    const expirationDate = new Date(0);
    expirationDate.setUTCSeconds(decodedToken.exp);
    return expirationDate;
  }

  isLoggedIn(): boolean {
    return !this.isTokenExpired();
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.status == 400) {
      // client-side error
      errorMessage = error.error.title;
    } else if (error.status == 500) {
      // server-side error
      errorMessage = error.error.msg;
    } else if (error.status == 0){
      // API down
      errorMessage = 'API is down';
    }
    
    alert(errorMessage);
    return throwError(() => errorMessage);
  }
}

