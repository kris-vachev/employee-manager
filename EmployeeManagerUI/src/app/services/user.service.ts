import { Injectable } from '@angular/core';
import { User } from '../interfaces/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly userLocalStorageKey = 'user';

  setUser(user: User): void {
    const userJson = JSON.stringify(user);

    localStorage.setItem(this.userLocalStorageKey, userJson);
  }

  getUser(): User | null {
    const userJson = localStorage.getItem(this.userLocalStorageKey);

    if (userJson) {
      return JSON.parse(userJson) as User;
    }

    return null;
  }

  clearUser(): void {
    localStorage.removeItem(this.userLocalStorageKey);
  }
}