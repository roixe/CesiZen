import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Article } from '../models/article';
import { Categorie } from '../models/categorie';

@Injectable({ providedIn: 'root' })
export class InfosService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<Categorie[]> {
    return this.http.get<Categorie[]>(`${this.baseUrl}/categories`);
  }

  getPublicArticles(): Observable<Article[]> {
    return this.http.get<Article[]>(`${this.baseUrl}/articles?public=true`);
  }

  getArticleById(id: number): Observable<Article> {
    return this.http.get<Article>(`${this.baseUrl}/articles/${id}`);
  }
}