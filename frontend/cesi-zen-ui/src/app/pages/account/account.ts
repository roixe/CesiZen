import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account.html'
})
export class AccountComponent {
  private readonly baseUrl = environment.apiBaseUrl;

  user = computed(() => this.auth.user());

  loading = signal(false);
  message = signal<string | undefined>(undefined);
  confirmDelete = signal(false);

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private router: Router
  ) {}

  // [RGPD] Droit à la portabilité : télécharge les données personnelles en JSON
  exportData(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.http.get(`${this.baseUrl}/user/me/export`).subscribe({
      next: (data) => {
        this.loading.set(false);
        const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'mes-donnees-cesizen.json';
        a.click();
        URL.revokeObjectURL(url);
        this.message.set('Vos données ont été exportées (fichier JSON téléchargé).');
      },
      error: () => {
        this.loading.set(false);
        this.message.set("Erreur lors de l'export des données.");
      }
    });
  }

  // [RGPD] Droit à l'effacement : suppression définitive du compte
  askDelete(): void {
    this.confirmDelete.set(true);
    this.message.set(undefined);
  }

  cancelDelete(): void {
    this.confirmDelete.set(false);
  }

  deleteAccount(): void {
    this.loading.set(true);

    this.http.delete(`${this.baseUrl}/user/me`).subscribe({
      next: () => {
        this.loading.set(false);
        this.auth.logout();
        this.router.navigate(['/']);
      },
      error: () => {
        this.loading.set(false);
        this.message.set('Erreur lors de la suppression du compte.');
      }
    });
  }
}
