import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminArticlesService, ArticleDto, UpsertArticleDto } from '../../services/admin-articles.service';
import { Observable, of, catchError, finalize, timeout } from 'rxjs';
import { AdminCategoriesService, CategorieDto } from '../../services/admin-categorie.service';


@Component({
  selector: 'app-admin-articles',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-articles.html'
})
export class AdminArticlesComponent implements OnInit {
  articles = signal<ArticleDto[]>([]);
  loading = signal(false);
  message = signal<string | undefined>(undefined);
  categories = signal<CategorieDto[]>([]);
  

  // Form state
  editId = signal<number | null>(null);
  titre = signal('');
  contenu = signal('');
  categorieId = signal<number>(1);
  isPublic = signal<boolean>(true);

  constructor(private admin: AdminArticlesService, private categoriesService: AdminCategoriesService) {}

  ngOnInit(): void {
    this.loadCategories();
    this.load();
  }
  articleSearch = '';

filteredArticles() {
  const q = this.articleSearch.trim().toLowerCase();

  if (!q) return this.articles();

  return this.articles().filter(a =>
    (a.titre ?? '').toLowerCase().includes(q) ||
    this.categoryName(a.categorieId).toLowerCase().includes(q)
  );
}

categoryName(categorieId: number): string {
  const category = this.categories().find(c => c.id === categorieId);
  return category?.nom ?? `#${categorieId}`;
}

  loadCategories(): void {
  this.categoriesService.getAll().pipe(
    timeout(5000),
    catchError(err => {
      this.message.set(`Erreur chargement catégories (status=${err?.status ?? 'n/a'})`);
      return of([] as CategorieDto[]);
    })
  ).subscribe(list => {
    this.categories.set(list);

    // Si on est en création et qu'on n'a pas de catégorie, on prend la 1ère
    if (!this.editId() && list.length > 0 && (!this.categorieId() || this.categorieId() < 1)) {
      this.categorieId.set(list[0].id);
    }
  });
  }

  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.admin.getAll().pipe(
      timeout(5000),
      catchError(err => {
        this.message.set(`Erreur chargement articles (status=${err?.status ?? 'n/a'})`);
        return of([] as ArticleDto[]);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(list => this.articles.set(list));
  }

  startCreate(): void {
    this.editId.set(null);
    this.titre.set('');
    this.contenu.set('');
    const first = this.categories()[0];
    this.categorieId.set(first ? first.id : 1);    
    this.isPublic.set(true);
    this.message.set(undefined);
  }

  startEdit(a: ArticleDto): void {
    this.editId.set(a.id);
    this.titre.set(a.titre);
    this.contenu.set(a.contenu);
    this.categorieId.set(a.categorieId);
    this.isPublic.set(a.public);
    this.message.set(undefined);
  }

  save(): void {
    const dto: UpsertArticleDto = {
      titre: this.titre().trim(),
      contenu: this.contenu(),
      categorieId: Number(this.categorieId()),
      public: !!this.isPublic()
    };

    if (!dto.titre || !dto.contenu || !dto.categorieId) {
      this.message.set('Champs requis : titre, contenu, categorieId.');
      return;
    }

    this.loading.set(true);
    this.message.set(undefined);

    const id = this.editId();
    const req$: Observable<unknown> = id != null
      ? this.admin.update(id, dto)
      : this.admin.create(dto);

    req$.pipe(
      timeout(5000),
      catchError(err => {
        this.message.set(`Erreur sauvegarde (status=${err?.status ?? 'n/a'})`);
        return of(null);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: () => {
        this.message.set(id ? 'Article modifié.' : 'Article créé.');
        this.startCreate();
        this.load();
      }
    });
  }

  delete(id: number): void {
    if (!confirm(`Supprimer l'article #${id} ?`)) return;

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
      this.message.set('Article supprimé.');
      if (this.editId() === id) this.startCreate();
      this.load();
    });
  }
}