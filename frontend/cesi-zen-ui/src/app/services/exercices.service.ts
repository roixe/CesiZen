import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Exercice } from '../models/exercice';

@Injectable({ providedIn: 'root' })
export class ExercicesService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

  getRespirationExercices(): Observable<Exercice[]> {
    return this.http.get<Exercice[]>(`${this.baseUrl}/exercices?type=RESPIRATION&public=true`);
  }

  getExerciceById(id: number): Observable<Exercice> {
    return this.http.get<Exercice>(`${this.baseUrl}/exercices/${id}`);
  }
}