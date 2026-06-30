import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HistoriquesService } from '../../services/historiques.service';
import { Historique } from '../../models/historique';
import { finalize, timeout, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './history.html'
})
export class HistoryComponent implements OnInit {
  userId = 1;

  historiques = signal<Historique[]>([]);
  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private historiquesService: HistoriquesService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.historiquesService.getMyHistorique().pipe(
      timeout(5000),
      catchError(err => {
        console.error('Historiques error:', err);
        this.message.set(`Erreur historiques (status=${err?.status ?? 'n/a'})`);
        return of([] as Historique[]);
      }),
      finalize(() => this.loading.set(false))
      ).subscribe((data: Historique[]) => {
       this.historiques.set(data);
      });
  }
}