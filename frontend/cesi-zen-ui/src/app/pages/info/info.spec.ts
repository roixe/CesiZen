import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { provideRouter } from '@angular/router';
import { InfoComponent } from './info';
import { InfosService } from '../../services/infos.service';
import { Article } from '../../models/article';
import { Categorie } from '../../models/categorie';

describe('InfoComponent', () => {
  let component: InfoComponent;
  let fixture: ComponentFixture<InfoComponent>;
  let infosServiceSpy: {
    getCategories: ReturnType<typeof vi.fn>;
    getPublicArticles: ReturnType<typeof vi.fn>;
    getArticleById: ReturnType<typeof vi.fn>;
  };

  const mockCategories: Categorie[] = [
    { id: 1, nom: 'Stress' },
    { id: 2, nom: 'Respiration' }
  ];

  const mockArticles: Article[] = [
    {
      id: 1,
      titre: 'Comprendre le stress',
      contenu: 'Le stress est une réaction naturelle.',
      datePublication: '2025-01-01',
      public: true,
      categorieId: 1
    },
    {
      id: 2,
      titre: 'Exercice 5-5',
      contenu: 'La respiration aide à se recentrer.',
      datePublication: '2025-01-02',
      public: true,
      categorieId: 2
    }
  ];

  beforeEach(async () => {
  infosServiceSpy = {
    getCategories: vi.fn().mockReturnValue(of(mockCategories)),
    getPublicArticles: vi.fn().mockReturnValue(of(mockArticles)),
    getArticleById: vi.fn()
  };

  await TestBed.configureTestingModule({
    imports: [InfoComponent],
    providers: [
      provideRouter([]),
      { provide: InfosService, useValue: infosServiceSpy }
    ]
  }).compileComponents();

  fixture = TestBed.createComponent(InfoComponent);
  component = fixture.componentInstance;
});

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load categories and articles on init', () => {
    fixture.detectChanges();

    expect(infosServiceSpy.getCategories).toHaveBeenCalled();
    expect(infosServiceSpy.getPublicArticles).toHaveBeenCalled();

    expect(component.categories().length).toBe(2);
    expect(component.articles().length).toBe(2);
    expect(component.loading()).toBe(false);
    expect(component.message()).toBeUndefined();
  });

  it('should return all articles when searchTerm is empty', () => {
    component.articles.set(mockArticles);
    component.categories.set(mockCategories);
    component.searchTerm = '';

    const result = component.filteredArticles();

    expect(result.length).toBe(2);
  });

  it('should filter articles by title', () => {
    component.articles.set(mockArticles);
    component.categories.set(mockCategories);
    component.searchTerm = 'stress';

    const result = component.filteredArticles();

    expect(result.length).toBe(1);
    expect(result[0].titre).toBe('Comprendre le stress');
  });

  it('should filter articles by content', () => {
    component.articles.set(mockArticles);
    component.categories.set(mockCategories);
    component.searchTerm = 'recentrer';

    const result = component.filteredArticles();

    expect(result.length).toBe(1);
    expect(result[0].titre).toBe('Exercice 5-5');
  });

  it('should filter articles by category name', () => {
    component.articles.set(mockArticles);
    component.categories.set(mockCategories);
    component.searchTerm = 'respiration';

    const result = component.filteredArticles();

    expect(result.length).toBe(1);
    expect(result[0].categorieId).toBe(2);
  });

  it('should return category name for a known category id', () => {
    component.categories.set(mockCategories);

    expect(component.categoryName(1)).toBe('Stress');
    expect(component.categoryName(2)).toBe('Respiration');
  });

  it('should return fallback when category id is unknown', () => {
    component.categories.set(mockCategories);

    expect(component.categoryName(999)).toBe('—');
  });

  it('should set message when categories loading fails', () => {
    infosServiceSpy.getCategories.mockReturnValue(
      throwError(() => ({ status: 500 }))
    );
    infosServiceSpy.getPublicArticles.mockReturnValue(of(mockArticles));

    fixture = TestBed.createComponent(InfoComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();

    expect(component.message()).toContain('Erreur catégories');
    expect(component.loading()).toBe(false);
    expect(component.articles().length).toBe(2);
  });

  it('should set message when articles loading fails', () => {
    infosServiceSpy.getCategories.mockReturnValue(of(mockCategories));
    infosServiceSpy.getPublicArticles.mockReturnValue(
      throwError(() => ({ status: 500 }))
    );

    fixture = TestBed.createComponent(InfoComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();

    expect(component.message()).toContain('Erreur articles');
    expect(component.loading()).toBe(false);
    expect(component.categories().length).toBe(2);
  });
});