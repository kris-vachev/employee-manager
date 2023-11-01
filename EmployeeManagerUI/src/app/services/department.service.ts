import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http'
import { environment } from 'src/environments/environment';
import { Observable, catchError, throwError } from 'rxjs';
import { ApiResponse } from '../interfaces/api-response';
import { Department } from '../interfaces/department';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private endpoint: string = environment.endpoint;
  private apiUrl: string = this.endpoint + 'api/department';

  constructor(private http:HttpClient) { }

  getDepartments(filterValue?: string, pageNumber?: number, pageSize?: number, sortField?: string, sortDirection?: string):Observable<ApiResponse>{
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

  getDepartmentList():Observable<ApiResponse>{
    return this.http.get<ApiResponse>(`${this.apiUrl}/list`).pipe(
      catchError(this.handleError)
    );
  }

  addDepartment(employee:Department):Observable<ApiResponse>{
    return this.http.post<ApiResponse>(this.apiUrl, employee).pipe(
      catchError(this.handleError)
    );
  }

  updateDepartment(employee:Department):Observable<ApiResponse>{
    return this.http.put<ApiResponse>(this.apiUrl, employee).pipe(
      catchError(this.handleError)
    );
  }

  deleteDepartment(departmentId:number):Observable<ApiResponse>{
    return this.http.delete<ApiResponse>(`${this.apiUrl}/${departmentId}`).pipe(
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
