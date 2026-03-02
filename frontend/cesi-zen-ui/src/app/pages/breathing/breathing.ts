import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExercicesService } from '../../services/exercices.service';
import { HistoriquesService } from '../../services/historiques.service';
import { Exercice } from '../../models/exercice';
import { timer } from 'rxjs';
import { retry } from 'rxjs/operators';

@Component({
  selector: 'app-breathing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './breathing.html'
})
export class BreathingComponent implements OnInit {
  exercices: Exercice[] = [];
  loading = true;
  message?: string;

  userId = 1; //temporaire

  constructor(
    private exercicesService: ExercicesService,
    private historiquesService: HistoriquesService
  ) {}

  ngOnInit(): void {
  this.loading = true;

  this.exercicesService.getRespirationExercices().pipe(
    retry({ count: 2, delay: () => timer(300) })
  ).subscribe({
    next: (data) => {
      this.exercices = data;
      this.loading = false;
      this.message = undefined;
    },
    error: (err) => {
      this.loading = false;
      this.message = `Erreur chargement exercices (status=${err?.status ?? 'n/a'})`;
    }
  });
  }

  startAndSave(ex: Exercice): void {
    this.historiquesService.createHistorique({
      utilisateurId: this.userId,
      exerciceId: ex.id,
      dureeEffectiveSec: ex.dureeTotaleSec
    }).subscribe({
      next: (res) => this.message = `Session enregistrée (historique id=${res.id})`,
      error: () => this.message = 'Erreur enregistrement historique.'
    });
  }
}