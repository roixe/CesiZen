import { Routes } from '@angular/router';
import { BreathingComponent } from './pages/breathing/breathing';
import { HistoryComponent } from './pages/history/history';

export const routes: Routes = [
  { path: '', redirectTo: 'breathing', pathMatch: 'full' },
  { path: 'breathing', component: BreathingComponent },
  { path: 'history', component: HistoryComponent }
];