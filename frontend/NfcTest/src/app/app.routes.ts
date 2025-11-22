import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'notas', pathMatch: 'full' },
  {
    path: 'notas',
    loadComponent: () => import('./notas/notas-list.component').then(m => m.NotasListComponent),
  },
  {
    path: 'exports/status/:jobId',
    loadComponent: () => import('./export/export-status.component').then(m => m.ExportStatusComponent),
  },
];
