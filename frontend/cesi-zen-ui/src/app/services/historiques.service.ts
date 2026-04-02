import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Historique } from '../models/historique';

export interface CreateHistoriqueRequest {
  exerciceId: number;
  dureeEffectiveSec: number;
}

@Injectable({ providedIn: 'root' })
export class HistoriquesService {
  private readonly baseUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) {}

getMyHistorique() {
  return this.http.get<Historique[]>(`${this.baseUrl}/historiques/me`);
}

createHistorique(req: { exerciceId: number; dureeEffectiveSec: number; }) {
  return this.http.post<{ id: number }>(`${this.baseUrl}/historiques`, req);
}
}