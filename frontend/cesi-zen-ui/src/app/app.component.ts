import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { HealthService, HealthStatus } from './services/health.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  title = 'CesiZen';
  healthStatus?: HealthStatus;
  errorMessage?: string;

  constructor(private healthService: HealthService) { }

  ngOnInit(): void {
    this.healthService.getHealth().subscribe({
      next: (status: HealthStatus) => {
        this.healthStatus = status;
      },
      error: (error: any) => {
        console.error(error);
        this.errorMessage = 'Impossible de contacter le backend.';
      }
    });
  }
}
