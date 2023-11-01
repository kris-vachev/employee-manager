import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DATE_FORMATS } from '@angular/material/core';
import { Department } from 'src/app/interfaces/department';
import { Employee } from 'src/app/interfaces/employee';
import { DepartmentService } from 'src/app/services/department.service';
import { EmployeeService } from 'src/app/services/employee.service';

@Component({
  selector: 'app-dialog-department-add-edit',
  templateUrl: './dialog-department-add-edit.component.html',
  styleUrls: ['./dialog-department-add-edit.component.css']
})
export class DialogDepartmentAddEditComponent implements OnInit {
  departmentForm: FormGroup;
  action: string = "Add Department";
  actionButton: string = "Add";

  constructor(
    private dialogReference: MatDialogRef<DialogDepartmentAddEditComponent>,
    @Inject(MAT_DIALOG_DATA) public departmentData: Department,
    private fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private _departmentService: DepartmentService,
    private _employeeService: EmployeeService
  ){
    this.departmentForm = this.fb.group({
      departmentName: ['',Validators.required],
    });
  }

  ngOnInit():void{
      if (this.departmentData){
        this.departmentForm.patchValue({
          departmentName: this.departmentData.departmentName,
        });

        this.action = "Edit Department";
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

  addEditDepartment(){
    const model: Department = {
      departmentId: this.departmentData == null ? 0 : this.departmentData.departmentId,
      departmentName: this.departmentForm.value.departmentName
    };

    if (this.departmentData == null){
      this._departmentService.addDepartment(model).subscribe({
        next: (data) => {
          if(data.status){
            this.showAlert('Department added!', 'Success');
            this.dialogReference.close('created');
          } else{
            this.showAlert('Could not add department!', 'Error');
          }
        },
        error: (e) => {}
      });
    } else{
      this._departmentService.updateDepartment(model).subscribe({
        next: (data) => {
          if(data.status){
            this.showAlert('Department updated!', 'Success');
            this.dialogReference.close('updated');
          } else{
            this.showAlert('Could not update department!', 'Error');
          }
        },
        error: (error) => {}
      });
    }
  }
}
