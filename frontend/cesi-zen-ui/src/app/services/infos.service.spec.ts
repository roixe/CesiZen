import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { InfosService } from './infos.service';
import { environment } from '../../environments/environment';
import { Article } from '../models/article';
import { Categorie } from '../models/categorie';

describe('InfosService', () => {
  let service: InfosService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        InfosService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(InfosService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get categories', () => {
    const mockCategories: Categorie[] = [
      { id: 1, nom: 'Stress' },
      { id: 2, nom: 'Respiration' }
    ];

    service.getCategories().subscribe(categories => {
      expect(categories.length).toBe(2);
      expect(categories[0].nom).toBe('Stress');
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/categories`);
    expect(req.request.method).toBe('GET');

    req.flush(mockCategories);
  });

  it('should get public articles', () => {
    const mockArticles: Article[] = [
      {
        id: 1,
        titre: 'Comprendre le stress',
        contenu: 'Contenu',
        datePublication: '2025-01-01',
        public: true,
        categorieId: 1
      }
    ];

    service.getPublicArticles().subscribe(articles => {
      expect(articles.length).toBe(1);
      expect(articles[0].public).toBeTruthy();
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/articles?public=true`);
    expect(req.request.method).toBe('GET');

    req.flush(mockArticles);
  });

  it('should get article by id', () => {
    const mockArticle: Article = {
      id: 1,
      titre: 'Comprendre le stress',
      contenu: 'Contenu',
      datePublication: '2025-01-01',
      public: true,
      categorieId: 1
    };

    service.getArticleById(1).subscribe(article => {
      expect(article.id).toBe(1);
      expect(article.titre).toBe('Comprendre le stress');
    });

    const req = httpMock.expectOne(`${environment.apiBaseUrl}/articles/1`);
    expect(req.request.method).toBe('GET');

    req.flush(mockArticle);
  });
});