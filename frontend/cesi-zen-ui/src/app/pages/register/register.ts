import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html'
})
export class RegisterComponent {
  nom = '';
  email = '';
  password = '';
  consentement = false; // [SÉCU 4] consentement RGPD

  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    // [SÉCU 4] on bloque tant que le consentement n'est pas coché
    if (!this.consentement) {
      this.message.set('Vous devez accepter le traitement de vos données (RGPD).');
      return;
    }

    this.loading.set(true);
    this.message.set(undefined);

    this.auth.register({
      nom: this.nom,
      email: this.email,
      password: this.password,
      consentement: this.consentement
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/breathing']);
      },
      error: (err) => {
        this.loading.set(false);

        if (err.status === 400) {
          this.message.set('Informations invalides');
        } else if (err.status === 0) {
          this.message.set('Serveur indisponible.');
        } else {
          this.message.set('Impossible de créer le compte.');
        }
      }
    });
  }
}
