import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login.page').then(m => m.LoginPage) },
  { path: 'pokemon-list', loadComponent: () => import('./pages/pokemon-list/pokemon-list.page').then(m => m.PokemonListPage), canActivate: [authGuard] },
  { path: 'pokemon-detail/:id', loadComponent: () => import('./pages/pokemon-detail/pokemon-detail.page').then(m => m.PokemonDetailPage), canActivate: [authGuard] },
  { path: 'tratamientos', loadComponent: () => import('./pages/tratamientos/tratamientos.page').then(m => m.TratamientosPage), canActivate: [authGuard] },
  { path: 'citas', loadComponent: () => import('./pages/citas/citas.page').then(m => m.CitasPage), canActivate: [authGuard] },
  { path: 'estado-clinico', loadComponent: () => import('./pages/estado-clinico/estado-clinico.page').then(m => m.EstadoClinicoPage), canActivate: [authGuard] },
  { path: 'comprobantes', loadComponent: () => import('./pages/comprobantes/comprobantes.page').then(m => m.ComprobantesPage), canActivate: [authGuard] }
];
