import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HistoriquesService } from '../../services/historiques.service';
import { Historique } from '../../models/historique';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './history.html'
})
export class HistoryComponent implements OnInit {
  userId = 1;
  historiques: Historique[] = [];
  loading = true;
  message?: string;

  constructor(private historiquesService: HistoriquesService) {}

  ngOnInit(): void {
    this.historiquesService.getHistoriqueByUser(this.userId).subscribe({
      next: (data) => { this.historiques = data; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }
}