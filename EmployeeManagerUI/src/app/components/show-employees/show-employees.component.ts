import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Employee } from 'src/app/interfaces/employee';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { EmployeeService } from 'src/app/services/employee.service';
import { DialogEmployeeAddEditComponent } from '../dialogs/dialog-employee-add-edit/dialog-employee-add-edit.component';
import { DialogEmployeeDeleteComponent } from '../dialogs/dialog-employee-delete/dialog-employee-delete.component';
import { MatSort } from '@angular/material/sort';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-show-employees',
  templateUrl: './show-employees.component.html',
  styleUrls: ['./show-employees.component.css']
})
export class ShowEmployeesComponent {
  displayedColumns: string[] = ['Name', 'Department', 'Salary', 'DateJoined', 'Actions'];
  employeeData = new MatTableDataSource<Employee>();

  @ViewChild(MatPaginator, { read: true }) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  
  pageNumber = 1;
  pageSize = 5;
  totalCount = 0;

  filterValue?: string;

  sortDirection: string = 'asc';

  constructor(
    private _snackBar: MatSnackBar,
    private _employeeService: EmployeeService,
    private dialog: MatDialog,
    private datePipe: DatePipe
  ){}

  formatDate(date: Date){
    return this.datePipe.transform(date,'yyyy/MM/dd');
  }

  sortData(event: any): void{
    this.sort.active = event.active;
    this.sort.direction = event.direction;
    this.sortDirection = event.direction;
    this.showEmployees(this.filterValue);
  }

  applyFilter(event:Event){
    const filterValue = (event.target as HTMLInputElement).value;
    this.filterValue = filterValue;
    this.showEmployees(this.filterValue);
  }

  showEmployees(filterValue?: string){
    this._employeeService.getEmployees(filterValue, this.pageNumber, this.pageSize, this.sort.active, this.sortDirection).subscribe({
      next:(data) => {
        if(data.status){
          this.employeeData.data = data.value;
          this.totalCount = data.value.length ? data.value[0].totalCount : 0;
        }
      },
      error: (e) => {}
    });
  }

  onPageChange(event: any): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.showEmployees(this.filterValue);
  }

  ngOnInit(): void{
    setTimeout(() => {
      this.showEmployees();
    }, 0);
  }

  ngAfterViewInit(): void {
    this.employeeData.paginator = this.paginator;
    this.employeeData.sort = this.sort;
    
    this.sort.sortChange.subscribe((event) => {
      this.sortData(event);
    });
  }
  
  addEmployee(){
    this.dialog.open(DialogEmployeeAddEditComponent, {
      disableClose: true,
      width: "350px"
    }).afterClosed().subscribe(result => {
      if (result == 'created') {
        this.showEmployees();
      }
    });
  }

  editEmployee(employee: Employee){
    this.dialog.open(DialogEmployeeAddEditComponent, {
      data: employee,
      disableClose: true,
      width: "350px"
    }).afterClosed().subscribe(result => {
      if (result == 'updated') {
        this.showEmployees();
      }
    });
  }

  deleteEmployee(employee: Employee){
    this.dialog.open(DialogEmployeeDeleteComponent, {
      data: employee,
      disableClose: true
    }).afterClosed().subscribe(result => {
      if (result == 'deleted'){
        this._employeeService.deleteEmployee(employee.employeeId).subscribe({
          next: (data) => {
            if (data.status){
              this.showAlert('Employee deleted!', 'Success');
              this.showEmployees();
            } else{
              this.showAlert('Could not delete employee!', 'Error');
            }
          },
          error: (error) => {}
        });
      }
    });
  }

  showAlert(msg: string, title: string){
    this._snackBar.open(msg,title, {
      horizontalPosition: "end",
      verticalPosition: "top",
      duration: 3000
    });
  }
}
