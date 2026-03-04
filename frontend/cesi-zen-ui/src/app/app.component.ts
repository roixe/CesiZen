import { Component, computed } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'CesiZen';

  isLoggedIn = computed(() => this.auth.isLoggedIn());

  constructor(private auth: AuthService) {}

  logout(): void {
    this.auth.logout();
  }
}