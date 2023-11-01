import { Department } from "./department";

export interface Employee {
    employeeId?: number;
    departmentId: number;
    employeeName: string;
    salary: number;
    dateJoined: string;
    department?: Department;
}
