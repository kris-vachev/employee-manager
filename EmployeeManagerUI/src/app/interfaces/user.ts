import { Role } from "./role";

export interface User {
    userId: number;
    userName: string;
    role: Role;
}