import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiResponse } from '../interfaces/api-response';
import { Employee } from '../interfaces/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService{
  private endpoint: string = environment.endpoint;
  private apiUrl: string = this.endpoint + 'api/employee';

  constructor(private http: HttpClient) { }

  getEmployees(filterValue?: string, pageNumber?: number, pageSize?: number, sortField?: string, sortDirection?: string): Observable<ApiResponse> {
    let params: any = {
      pageNumber: pageNumber,
      pageSize: pageSize
    };

    if (filterValue) {
      params.filterValue = filterValue;
    }

    if (sortField && sortDirection){
      params.sortField = sortField;
      params.sortDirection = sortDirection;
    }

    return this.http.get<ApiResponse>(this.apiUrl, { params }).pipe(
      catchError(this.handleError)
    );
  }

  addEmployee(employee: Employee): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(this.apiUrl, employee).pipe(
      catchError(this.handleError)
    );
  }

  updateEmployee(employee: Employee): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(this.apiUrl, employee).pipe(
      catchError(this.handleError)
    );
  }

  deleteEmployee(employeeId?: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${employeeId}`).pipe(
      catchError(this.handleError)
    );
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
