import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, AdminUser } from '../../services/admin.service';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-users.html'
})
export class AdminUsersComponent implements OnInit {
  users = signal<AdminUser[]>([]);
  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private admin: AdminService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.admin.getUsers().pipe(
      catchError(err => {
        this.message.set(`Erreur chargement users (status=${err?.status ?? 'n/a'})`);
        return of([] as AdminUser[]);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(data => this.users.set(data));
  }

  toggleActive(u: AdminUser): void {
    const req = u.actif ? this.admin.disableUser(u.id) : this.admin.enableUser(u.id);
    req.subscribe({
      next: () => this.load(),
      error: (err) => this.message.set(`Erreur update (status=${err?.status ?? 'n/a'})`)
    });
  }

  toggleRole(u: AdminUser): void {
    const nextRole = (u.role ?? '').toUpperCase() === 'ADMIN' ? 'USER' : 'ADMIN';
    this.admin.setRole(u.id, nextRole as 'ADMIN' | 'USER').subscribe({
      next: () => this.load(),
      error: (err) => this.message.set(`Erreur role (status=${err?.status ?? 'n/a'})`)
    });
  }
}