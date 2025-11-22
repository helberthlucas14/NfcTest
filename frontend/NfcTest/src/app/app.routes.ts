import { Routes } from '@angular/router';
import { NotasListComponent } from './notas/notas-list.component';

export const routes: Routes = [
      { path: '', redirectTo: 'notas', pathMatch: 'full' },
      { path: 'notas', component: NotasListComponent },
];
