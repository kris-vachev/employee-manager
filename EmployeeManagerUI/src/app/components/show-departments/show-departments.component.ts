import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Department } from 'src/app/interfaces/department';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentService } from 'src/app/services/department.service';
import { MatSort } from '@angular/material/sort';
import { DialogDepartmentAddEditComponent } from '../dialogs/dialog-department-add-edit/dialog-department-add-edit.component';
import { DialogDepartmentDeleteComponent } from '../dialogs/dialog-department-delete/dialog-department-delete.component';

@Component({
  selector: 'app-show-departments',
  templateUrl: './show-departments.component.html',
  styleUrls: ['./show-departments.component.css']
})
export class ShowDepartmentsComponent implements OnInit{
  displayedColumns: string[] = ['Name', 'Actions'];
  departmentData = new MatTableDataSource<Department>();

  @ViewChild(MatPaginator, { read: true }) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  
  pageNumber = 1;
  pageSize = 5;
  totalCount = 0;

  filterValue?: string;

  sortDirection: string = 'asc';

  constructor(
    private _snackBar: MatSnackBar,
    private _departmentService: DepartmentService,
    private dialog: MatDialog
  ){}

  applyFilter(event:Event){
    const filterValue = (event.target as HTMLInputElement).value;
    this.filterValue = filterValue;
    this.showDepartments(this.filterValue);
  }

  sortData(event: any): void{
    this.sort.active = event.active;
    this.sort.direction = event.direction;
    this.sortDirection = event.direction;
    this.showDepartments(this.filterValue);
  }

  showDepartments(filterValue?: string){
    this._departmentService.getDepartments(filterValue, this.pageNumber, this.pageSize, this.sort.active, this.sortDirection).subscribe({
      next:(data) => {
        if(data.status){
          this.departmentData.data = data.value;
          this.totalCount = data.value.length ? data.value[0].totalCount : 0;
        }
      },
      error: (e) => {}
    });
  }

  onPageChange(event: any): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.showDepartments(this.filterValue);
  }

  ngOnInit(): void{
    setTimeout(() => {
      this.showDepartments();
    }, 0);
  }

  ngAfterViewInit(): void {
    this.departmentData.paginator = this.paginator;
    this.departmentData.sort = this.sort;
    
    this.sort.sortChange.subscribe((event) => {
      this.sortData(event);
    });
  }
  
  addDepartment(){
    this.dialog.open(DialogDepartmentAddEditComponent, {
      disableClose: true,
      width: "350px"
    }).afterClosed().subscribe(result => {
      if (result == 'created') {
        this.showDepartments();
      }
    });
  }

  editDepartment(department: Department){
    this.dialog.open(DialogDepartmentAddEditComponent, {
      data: department,
      disableClose: true,
      width: "350px"
    }).afterClosed().subscribe(result => {
      if (result == 'updated') {
        this.showDepartments();
      }
    });
  }

  deleteDepartment(department: Department){
    this.dialog.open(DialogDepartmentDeleteComponent, {
      data: department,
      disableClose: true
    }).afterClosed().subscribe(result => {
      if (result == 'deleted'){
        this._departmentService.deleteDepartment(department.departmentId).subscribe({
          next: (data) => {
            if (data.status){
              this.showAlert('Department deleted!', 'Success');
              this.showDepartments();
            } else{
              this.showAlert('Could not delete department!', 'Error');
            }
          },
          error: (error) => {
            let errorMessage = 'An error occurred';
            if (error.status == 400) {
              // Client-side error
              errorMessage = error.error.message;
            } else if (error.status == 500) {
              // Server-side error
              errorMessage = error.error.msg || errorMessage;
            }
            this.showAlert(errorMessage, 'Error');
          }
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
