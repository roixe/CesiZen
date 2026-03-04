import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExercicesService } from '../../services/exercices.service';
import { HistoriquesService } from '../../services/historiques.service';
import { Exercice } from '../../models/exercice';
import { finalize, timeout, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-breathing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './breathing.html'
})
export class BreathingComponent implements OnInit {
  exercices = signal<Exercice[]>([]);
  loading = signal<boolean>(false);
  message = signal<string | undefined>(undefined);


  constructor(
    private exercicesService: ExercicesService,
    private historiquesService: HistoriquesService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.exercicesService.getRespirationExercices().pipe(
      timeout(5000),
      catchError(err => {
        console.error('Exercices error:', err);
        this.message.set(`Erreur chargement exercices (status=${err?.status ?? 'n/a'})`);
        return of([] as Exercice[]);
      }),
      finalize(() => {
        this.loading.set(false);
      })
    ).subscribe(data => {
      this.exercices.set(data);
    });
  }

  startAndSave(ex: Exercice): void {
    this.message.set('Enregistrement en cours...');
    
    this.historiquesService.createHistorique({
      exerciceId: ex.id,
      dureeEffectiveSec: ex.dureeTotaleSec
    }).pipe(
      timeout(5000),
      catchError(err => {
        console.error('Create historique error:', err);
        this.message.set(`Erreur enregistrement (status=${err?.status ?? 'n/a'})`);
        return of(null);
      })
    ).subscribe(res => {
      if (res) {
        this.message.set(`Session enregistrée (historique id=${res.id})`);
      }
    });
  }
}