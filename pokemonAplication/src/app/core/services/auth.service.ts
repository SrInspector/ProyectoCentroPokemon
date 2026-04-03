import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'http://localhost:5000/api';
  private tokenKey = 'auth_token';
  private userKey = 'auth_user';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<any> {
    return new Observable(observer => {
      setTimeout(() => {
        if (email === 'trainer@pokemon.com' && password === 'Trainer123!') {
          const fakeResponse = {
            token: 'fake-jwt-token-123',
            user: { id: 1, name: 'Ash Ketchum', email: email, role: 'Entrenador' }
          };
          observer.next(fakeResponse);
          observer.complete();
        } else {
          observer.error({ message: 'Credenciales incorrectas' });
        }
      }, 1000);
    });
  }

  saveSession(token: string, user: any): void {
    localStorage.setItem(this.tokenKey, token);
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.isAuthenticatedSubject.next(true);
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getUser(): any {
    const user = localStorage.getItem(this.userKey);
    return user ? JSON.parse(user) : null;
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}