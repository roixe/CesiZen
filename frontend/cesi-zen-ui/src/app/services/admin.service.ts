import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface AdminUser {
  id: number;
  nom: string;
  email: string;
  role: string;
  actif: boolean;
  dateCreation: string;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.baseUrl}/admin/users`);
  }

  disableUser(id: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/users/${id}/disable`, {});
  }

  enableUser(id: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/users/${id}/enable`, {});
  }

  setRole(id: number, role: 'ADMIN' | 'USER'): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/users/${id}/role`, role);
  }
}