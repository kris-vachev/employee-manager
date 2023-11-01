import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Employee } from 'src/app/interfaces/employee';

@Component({
  selector: 'app-dialog-delete',
  templateUrl: './dialog-employee-delete.component.html',
  styleUrls: ['./dialog-employee-delete.component.css']
})
export class DialogEmployeeDeleteComponent implements OnInit{

  constructor(
    private dialogReference: MatDialogRef<DialogEmployeeDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public employeeDelete: Employee
  ){}

  ngOnInit(): void {
    
  }

  confirmDelete(){
    if(this.employeeDelete){
      this.dialogReference.close('deleted');
    }
  }

}
