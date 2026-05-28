import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html'
})
export class LoginComponent {
  email = '';
  password = '';

  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private auth: AuthService, private router: Router) {}

  submit(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/breathing']);
      },
error: (err) => {
  this.loading.set(false);

  if (err.status === 401) {
    this.message.set("Email ou mot de passe incorrect.");
  } else if (err.status === 0) {
    this.message.set("Impossible de contacter le serveur.");
  } else {
    this.message.set("Connexion impossible.");
  }
}
    });
  }
}