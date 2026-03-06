import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest } from '../models/auth';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

const TOKEN_KEY = 'cz_token';
const USER_KEY = 'cz_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = environment.apiBaseUrl;

  token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  user = signal<AuthUser | null>(this.readUser());

  isLoggedIn = () => !!this.token();
  isAdmin = () => (this.user()?.role ?? '').toUpperCase() === 'ADMIN';

  constructor(private http: HttpClient) {}

  login(req: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/login`, req).pipe(
      tap(res => this.persistAuth(res))
    );
  }

  register(req: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/auth/register`, req).pipe(
      tap(res => this.persistAuth(res))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.token.set(null);
    this.user.set(null);
  }

  private persistAuth(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.token);
    localStorage.setItem(USER_KEY, JSON.stringify(res.user));
    this.token.set(res.token);
    this.user.set(res.user);
  }

  private readUser(): AuthUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) return null;
    try { return JSON.parse(raw) as AuthUser; } catch { return null; }
  }
}