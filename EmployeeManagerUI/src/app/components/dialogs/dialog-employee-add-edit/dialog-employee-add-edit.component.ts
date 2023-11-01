import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Department } from 'src/app/interfaces/department';
import { Employee } from 'src/app/interfaces/employee';
import { DepartmentService } from 'src/app/services/department.service';
import { EmployeeService } from 'src/app/services/employee.service';
import * as moment from 'moment';

export const DATE_FORMATS = {
  parse:{
    dateInput: 'YYYY/MM/DD'
  },
  display:{
    dateInput: 'YYYY/MM/DD',
    monthYearLabel: 'MMMM YYYY',
    dataA11yLabel: 'LL',
    monthYearA11Label:'MMMM YYYY'
  }
}

@Component({
  selector: 'app-dialog-add-edit',
  templateUrl: './dialog-employee-add-edit.component.html',
  styleUrls: ['./dialog-employee-add-edit.component.css'],
  providers: [
    { provide:MAT_DATE_FORMATS, useValue: DATE_FORMATS }
  ]
})
export class DialogEmployeeAddEditComponent implements OnInit {
  employeeForm: FormGroup;
  action: string = "Add Employee";
  actionButton: string = "Add";
  departmentList: Department[] = [];

  constructor(
    private dialogReference: MatDialogRef<DialogEmployeeAddEditComponent>,
    @Inject(MAT_DIALOG_DATA) public employeeData: Employee,
    private fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private _departmentService: DepartmentService,
    private _employeeService: EmployeeService
  ){
    this.employeeForm = this.fb.group({
      departmentId: ['',Validators.required],
      employeeName: ['',Validators.required],
      salary: ['',Validators.required],
      dateJoined: ['',Validators.required]
    });

    this._departmentService.getDepartmentList().subscribe({
      next: (data) => {
        if (data.status){
          this.departmentList = data.value;
        }
      },
      error: (e) => {}
    });
  }

  ngOnInit():void{
      if (this.employeeData){
        this.employeeForm.patchValue({
          departmentId: this.employeeData.departmentId,
          employeeName: this.employeeData.employeeName,
          salary: this.employeeData.salary,
          dateJoined: this.employeeData.dateJoined
        });

        this.action = "Edit Employee";
        this.actionButton = "Save";
      }
  }

  showAlert(msg: string, title: string){
    this._snackBar.open(msg,title, {
      horizontalPosition: "end",
      verticalPosition: "top",
      duration: 3000
    });
  }

  addEditEmployee(){
    const model: Employee = {
      employeeId: this.employeeData == null ? 0 : this.employeeData.employeeId,
      departmentId: this.employeeForm.value.departmentId,
      employeeName: this.employeeForm.value.employeeName,
      salary: this.employeeForm.value.salary,
      dateJoined: moment(this.employeeForm.value.dateJoined).format('YYYY-MM-DD')
    };

    if (this.employeeData == null){
      this._employeeService.addEmployee(model).subscribe({
        next: (data) => {
          if(data.status){
            this.showAlert('Employee added!', 'Success');
            this.dialogReference.close('created');
          } else{
            this.showAlert('Could not add employee!', 'Error');
          }
        },
        error: (e) => {}
      });
    } else{
      this._employeeService.updateEmployee(model).subscribe({
        next: (data) => {
          if(data.status){
            this.showAlert('Employee updated!', 'Success');
            this.dialogReference.close('updated');
          } else{
            this.showAlert('Could not update employee!', 'Error');
          }
        },
        error: (error) => {}
      });
    }
    
  }
}
