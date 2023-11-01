import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Department } from 'src/app/interfaces/department';

@Component({
  selector: 'app-dialog-department-delete',
  templateUrl: './dialog-department-delete.component.html',
  styleUrls: ['./dialog-department-delete.component.css']
})
export class DialogDepartmentDeleteComponent implements OnInit {
  constructor(
    private dialogReference: MatDialogRef<DialogDepartmentDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public departmentDelete: Department
  ){}

  ngOnInit(): void {
    
  }

  confirmDelete(){
    if(this.departmentDelete){
      this.dialogReference.close('deleted');
    }
  }
}
