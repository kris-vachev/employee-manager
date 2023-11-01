import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { AccountService } from '../services/account.service';
import { switchMap, catchError, filter, take } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private refreshTokenInProgress = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private _accountService: AccountService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // get token
    const token = localStorage.getItem('token');

    if (token) {
      const isTokenExpired = this._accountService.isTokenExpired();
      // refresh token
      if (isTokenExpired) {
        // keep track if token is refreshed
        if (!this.refreshTokenInProgress) {
          this.refreshTokenInProgress = true;
          this.refreshTokenSubject.next(null);

          // call service
          return this._accountService.refreshToken().pipe(
            switchMap((newToken: string) => {
              this.refreshTokenInProgress = false;
              this.refreshTokenSubject.next(newToken);

              const authRequest = request.clone({
                headers: request.headers.set('Authorization', `Bearer ${newToken}`)
              });

              return next.handle(authRequest);
            }),
            catchError((error) => {
              this.refreshTokenInProgress = false;
              return throwError(() => error);
            })
          );
        } else {
          // send expired token
          const authRequest = request.clone({
            setHeaders: { Authorization: `Bearer ${token}` }
          });

          return next.handle(authRequest);
        }
      } else {
        // send token
        const authRequest = request.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });

        return next.handle(authRequest);
      }
    }

    return next.handle(request);
  }
}