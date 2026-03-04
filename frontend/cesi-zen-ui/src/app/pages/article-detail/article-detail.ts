import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { InfosService } from '../../services/infos.service';
import { Article } from '../../models/article';

@Component({
  selector: 'app-article-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './article-detail.html'
})
export class ArticleDetailComponent implements OnInit {
  article = signal<Article | null>(null);
  loading = signal(false);
  message = signal<string | undefined>(undefined);

  constructor(private route: ActivatedRoute, private infos: InfosService) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.message.set('Article introuvable.');
      return;
    }

    this.loading.set(true);
    this.infos.getArticleById(id).subscribe({
      next: (a) => { this.article.set(a); this.loading.set(false); },
      error: (err) => { this.loading.set(false); this.message.set(`Erreur (status=${err?.status ?? 'n/a'})`); }
    });
  }
}