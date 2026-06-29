import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminCategoriesService, CategorieDto } from '../../services/admin-categorie.service';
import { catchError, finalize, timeout } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories.html'
})
export class AdminCategoriesComponent implements OnInit {
  categories = signal<CategorieDto[]>([]);
  loading = signal(false);
  message = signal<string | undefined>(undefined);

  editId: number | null = null;
  nom = '';

  constructor(private admin: AdminCategoriesService) {}

  ngOnInit(): void { this.load(); }
categorySearch = '';

filteredCategories() {
  const q = this.categorySearch.trim().toLowerCase();

  if (!q) return this.categories();

  return this.categories().filter(c =>
    (c.nom ?? '').toLowerCase().includes(q)
  );
}
  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.admin.getAll().pipe(
      timeout(5000),
      catchError(err => {
        this.message.set(`Erreur chargement catégories (status=${err?.status ?? 'n/a'})`);
        return of([] as CategorieDto[]);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(list => this.categories.set(list));
  }

  startCreate(): void {
    this.editId = null;
    this.nom = '';
    this.message.set(undefined);
  }

  startEdit(c: CategorieDto): void {
    this.editId = c.id;
    this.nom = c.nom;
    this.message.set(undefined);
  }

  save(): void {
    const name = this.nom.trim();
    if (name.length < 2) { this.message.set('Nom trop court.'); return; }

    this.loading.set(true);
    this.message.set(undefined);
    
    const id = this.editId;
    const req$: Observable<unknown> = id != null
      ? this.admin.update(id, { nom: name })
      : this.admin.create({ nom: name });

    req$.pipe(
      timeout(5000),
      catchError(err => {
        this.message.set(`Erreur sauvegarde (status=${err?.status ?? 'n/a'})`);
        return of(null);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(() => {
      this.message.set(this.editId ? 'Catégorie modifiée.' : 'Catégorie créée.');
      this.startCreate();
      this.load();
    });
  }

  delete(id: number): void {
    if (!confirm(`Supprimer la catégorie #${id} ?`)) return;

    this.loading.set(true);
    this.message.set(undefined);

    this.admin.delete(id).pipe(
      timeout(5000),
      catchError(err => {
        this.message.set(`Erreur suppression (status=${err?.status ?? 'n/a'})`);
        return of(null);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(() => {
      this.message.set('Catégorie supprimée.');
      if (this.editId === id) this.startCreate();
      this.load();
    });
  }
}