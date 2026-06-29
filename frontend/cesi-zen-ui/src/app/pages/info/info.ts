import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { InfosService } from '../../services/infos.service';
import { Article } from '../../models/article';
import { Categorie } from '../../models/categorie';
import { catchError, finalize, timeout } from 'rxjs/operators';
import { of, forkJoin } from 'rxjs';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-info',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './info.html'
})
export class InfoComponent implements OnInit {
  loading = signal(false);
  message = signal<string | undefined>(undefined);

  articles = signal<Article[]>([]);
  categories = signal<Categorie[]>([]);

  // map id -> nom
  categorieNameById = computed(() => {
    const map = new Map<number, string>();
    for (const c of this.categories()) map.set(c.id, c.nom);
    return map;
  });

constructor(private infosService: InfosService) {}
searchTerm = '';

filteredArticles() {
  const q = this.searchTerm.trim().toLowerCase();

  if (!q) return this.articles();

  return this.articles().filter(a =>
    (a.titre ?? '').toLowerCase().includes(q) ||
    (a.contenu ?? '').toLowerCase().includes(q) ||
    this.categoryName(a.categorieId).toLowerCase().includes(q)
  );
}
  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.message.set(undefined);

    forkJoin({
      categories: this.infosService.getCategories().pipe(
        timeout(5000),
        catchError(err => {
          console.error('Categories error', err);
          this.message.set(`Erreur catégories (status=${err?.status ?? 'n/a'})`);
          return of([] as Categorie[]);
        })
      ),
      articles: this.infosService.getPublicArticles().pipe(
        timeout(5000),
        catchError(err => {
          console.error('Articles error', err);
          this.message.set(`Erreur articles (status=${err?.status ?? 'n/a'})`);
          return of([] as Article[]);
        })
      )
    }).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe(({ categories, articles }) => {
      this.categories.set(categories);
      this.articles.set(articles);
    });
  }

  categoryName(id: number): string {
    return this.categorieNameById().get(id) ?? '—';
  }
}