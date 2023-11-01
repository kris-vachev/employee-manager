import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';

import { MomentDateModule } from '@angular/material-moment-adapter';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatGridListModule } from '@angular/material/grid-list';
import { DepartmentService } from './services/department.service';
import { EmployeeService } from './services/employee.service';
import { DialogEmployeeAddEditComponent } from './components/dialogs/dialog-employee-add-edit/dialog-employee-add-edit.component';
import { DialogEmployeeDeleteComponent } from './components/dialogs/dialog-employee-delete/dialog-employee-delete.component';
import { MatSortModule } from '@angular/material/sort';
import { DatePipe } from '@angular/common';
import { ShowEmployeesComponent } from './components/show-employees/show-employees.component';
import { ShowDepartmentsComponent } from './components/show-departments/show-departments.component';
import { DialogDepartmentAddEditComponent } from './components/dialogs/dialog-department-add-edit/dialog-department-add-edit.component';
import { DialogDepartmentDeleteComponent } from './components/dialogs/dialog-department-delete/dialog-department-delete.component';
import { HeaderComponent } from './components/navigation/header/header.component';
import { LoginComponent } from './components/login/login.component';

import { AuthInterceptor } from './helpers/auth.interceptor';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';

@NgModule({
  declarations: [
    AppComponent,
    DialogEmployeeAddEditComponent,
    DialogEmployeeDeleteComponent,
    ShowEmployeesComponent,
    ShowDepartmentsComponent,
    DialogDepartmentAddEditComponent,
    DialogDepartmentDeleteComponent,
    HeaderComponent,
    LoginComponent,
    UnauthorizedComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MomentDateModule,
    MatSnackBarModule,
    MatIconModule,
    MatDialogModule,
    MatGridListModule,
    MatSortModule,
    MatSidenavModule,
    MatToolbarModule,
    MatCardModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    DepartmentService, 
    EmployeeService,
    DatePipe,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }