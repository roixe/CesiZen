import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface CategorieDto { id: number; nom: string; }
export interface UpsertCategorieDto { nom: string; }

@Injectable({ providedIn: 'root' })
export class AdminCategoriesService {
  private readonly baseUrl = environment.apiBaseUrl;
  constructor(private http: HttpClient) {}

  getAll(): Observable<CategorieDto[]> {
    return this.http.get<CategorieDto[]>(`${this.baseUrl}/admin/categories`);
  }
  create(dto: UpsertCategorieDto): Observable<CategorieDto> {
    return this.http.post<CategorieDto>(`${this.baseUrl}/admin/categories`, dto);
  }
  update(id: number, dto: UpsertCategorieDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/categories/${id}`, dto);
  }
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/admin/categories/${id}`);
  }
}