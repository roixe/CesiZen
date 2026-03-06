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

  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.auth.register({ nom: this.nom, email: this.email, password: this.password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/breathing']);
      },
      error: (err) => {
        this.loading.set(false);
        this.message.set(`Register échoué (status=${err?.status ?? 'n/a'})`);
      }
    });
  }
}