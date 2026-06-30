import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface ArticleDto {
  id: number;
  titre: string;
  contenu: string;
  datePublication?: string | null;
  public: boolean;
  categorieId: number;
}

export interface UpsertArticleDto {
  titre: string;
  contenu: string;
  categorieId: number;
  public: boolean;
}

@Injectable({ providedIn: 'root' })
export class AdminArticlesService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ArticleDto[]> {
    return this.http.get<ArticleDto[]>(`${this.baseUrl}/admin/articles`);
  }

  create(dto: UpsertArticleDto): Observable<ArticleDto> {
    return this.http.post<ArticleDto>(`${this.baseUrl}/admin/articles`, dto);
  }

  update(id: number, dto: UpsertArticleDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/admin/articles/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/admin/articles/${id}`);
  }
}