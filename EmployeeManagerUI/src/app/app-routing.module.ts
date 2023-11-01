import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShowDepartmentsComponent } from './components/show-departments/show-departments.component';
import { ShowEmployeesComponent } from './components/show-employees/show-employees.component';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from './helpers/auth.guard';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { LoginGuard } from './helpers/login.guard';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
  { path: 'employees', component: ShowEmployeesComponent, canActivate: [AuthGuard] },
  { path: 'departments', component: ShowDepartmentsComponent, canActivate: [AuthGuard], data: { roles: ['Administrator'] } },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: '**', redirectTo: 'employees', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
