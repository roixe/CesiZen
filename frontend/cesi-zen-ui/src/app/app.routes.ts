import { Routes } from '@angular/router';
import { BreathingComponent } from './pages/breathing/breathing';
import { HistoryComponent } from './pages/history/history';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { authGuard } from './guards/auth.guard';

import { InfoComponent } from './pages/info/info';
import { ArticleDetailComponent } from './pages/article-detail/article-detail';
import { AdminUsersComponent } from './pages/admin-users/admin-users';
import { adminGuard } from './guards/admin.guard';
import { AdminArticlesComponent } from './pages/admin-articles/admin-articles';
import { AdminCategoriesComponent } from './pages/admin-categories/admin-categories';
import { AccountComponent } from './pages/account/account';



export const routes: Routes = [
  { path: '', redirectTo: 'breathing', pathMatch: 'full' },

  { path: 'breathing', component: BreathingComponent },
  { path: 'history', component: HistoryComponent, canActivate: [authGuard] },

  { path: 'info', component: InfoComponent },
  { path: 'info/:id', component: ArticleDetailComponent },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  { path: 'account', component: AccountComponent, canActivate: [authGuard] },

  { path: 'admin/users', component: AdminUsersComponent, canActivate: [adminGuard] },
  { path: 'admin/articles', component: AdminArticlesComponent, canActivate: [adminGuard] },
  { path: 'admin/categories', component: AdminCategoriesComponent, canActivate: [adminGuard] },
  { path: '**', redirectTo: 'info' }
];
