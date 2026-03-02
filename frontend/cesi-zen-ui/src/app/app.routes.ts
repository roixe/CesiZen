import { Routes } from '@angular/router';
import { BreathingComponent } from './pages/breathing/breathing';
import { HistoryComponent } from './pages/history/history';
import { InfoComponent } from './pages/info/info';
import { ArticleDetailComponent } from './pages/article-detail/article-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'breathing', pathMatch: 'full' },
  { path: 'breathing', component: BreathingComponent },
  { path: 'history', component: HistoryComponent },
  { path: 'info', component: InfoComponent },
  { path: 'info/:id', component: ArticleDetailComponent },
  
];