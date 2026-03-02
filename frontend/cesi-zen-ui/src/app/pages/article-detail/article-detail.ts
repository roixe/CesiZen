import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { InfosService } from '../../services/infos.service';
import { Article } from '../../models/article';
import { catchError, finalize, timeout } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-article-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './article-detail.html'
})
export class ArticleDetailComponent implements OnInit {
  loading = signal(false);
  message = signal<string | undefined>(undefined);
  article = signal<Article | null>(null);

  constructor(private route: ActivatedRoute, private infosService: InfosService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id || Number.isNaN(id)) {
      this.message.set('Identifiant article invalide.');
      return;
    }
    this.load(id);
  }

  load(id: number): void {
    this.loading.set(true);
    this.message.set(undefined);

    this.infosService.getArticleById(id).pipe(
      timeout(5000),
      catchError(err => {
        console.error('Article detail error', err);
        this.message.set(`Article introuvable (status=${err?.status ?? 'n/a'})`);
        return of(null);
      }),
      finalize(() => this.loading.set(false))
    ).subscribe(a => this.article.set(a));
  }
}