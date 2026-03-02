import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { HealthService, HealthStatus } from './services/health.service';
import { RouterOutlet, RouterLink } from '@angular/router';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule, RouterOutlet, RouterLink],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  title = 'CesiZen';
  healthStatus?: HealthStatus;
  errorMessage?: string;

  loading = true;

  constructor(private healthService: HealthService) { }

checkBackend(): void {
  this.loading = true;
  this.healthStatus = undefined;
  this.errorMessage = undefined;

  this.healthService.getHealth().subscribe({
    next: (status: HealthStatus) => {
      this.healthStatus = status;
      this.loading = false;
    },
    error: (error: any) => {
      this.errorMessage = `Backend indisponible (status=${error?.status ?? 'n/a'})`;
      this.loading = false;
    }
  });
}

ngOnInit(): void {
  this.checkBackend();
}
}
