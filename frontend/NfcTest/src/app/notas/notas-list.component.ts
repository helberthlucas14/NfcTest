import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatTableDataSource } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../core/api.service';
import { NotaFiscal, ApiMeta } from '../core/models';
import { EditNotaDialogComponent } from './edit-nota-dialog.component';

@Component({
    selector: 'app-notas-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        MatTableModule,
        MatCheckboxModule,
        MatButtonModule,
        MatIconModule,
        MatToolbarModule,
        MatCardModule,
        MatSnackBarModule,
        MatPaginatorModule,
        MatProgressBarModule,
    ],
    templateUrl: './notas-list.component.html',
})
export class NotasListComponent {
    private api = inject(ApiService);
    private dialog = inject(MatDialog);
    private snack = inject(MatSnackBar);


    displayedColumns = ['select', 'id', 'emissor', 'data', 'valor', 'acoes'];
    dataSource = new MatTableDataSource<NotaFiscal>([]);
    meta?: ApiMeta;
    pageSize = 10;
    isExporting = false;
    selectedIds = new Set<number>();

    ngOnInit() {
        this.load(1);
    }

    load(page: number = 1) {
        this.api.listNotas(page, this.pageSize).subscribe({
            next: (resp) => { this.dataSource.data = resp.data || []; this.meta = resp.meta; },
            error: () => { this.dataSource.data = []; this.meta = undefined; },
        });
    }

    onPage(e: PageEvent) {
        this.pageSize = e.pageSize;
        this.load(e.pageIndex + 1);
    }

    novaNota() {
        const ref = this.dialog.open(EditNotaDialogComponent, { data: {} });
        ref.afterClosed().subscribe(result => {
            if (!result) return;
            this.api.createNota({ emissor: result.emissor, dataEmissao: result.dataEmissao, items: result.items || [] })
                .subscribe({
                    next: () => { this.snack.open('Nota criada', 'OK', { duration: 2000 }); this.load(1); },
                    error: () => this.snack.open('Erro ao criar nota', 'OK', { duration: 3000 })
                });
        });
    }

      editar(nota: NotaFiscal) {
    const ref = this.dialog.open(EditNotaDialogComponent, { data: { nota } });
    ref.afterClosed().subscribe(result => {
      if (!result) return;
      const req = { emissor: result.emissor, dataEmissao: result.dataEmissao, items: result.items || [] };
      this.api.updateNota(nota.id, req).subscribe({
        next: () => { this.snack.open('Nota atualizada', 'OK', { duration: 2000 }); this.load(1); },
        error: () => this.snack.open('Erro ao atualizar nota', 'OK', { duration: 3000 })
      });
    });
  }

  excluir(nota: NotaFiscal) {
    this.api.deleteNota(nota.id).subscribe({
      next: () => { this.snack.open('Nota excluída', 'OK', { duration: 2000 }); this.load(1); },
      error: () => this.snack.open('Erro ao excluir nota', 'OK', { duration: 3000 })
    });
  }

  isSelected(id: number): boolean {
        return this.selectedIds.has(id);
    }

    toggleRow(id: number, checked: boolean): void {
        if (checked) this.selectedIds.add(id); else this.selectedIds.delete(id);
    }

    clearSelection(): void {
        this.selectedIds.clear();
    }

    selectAllCurrentPage(): void {
        (this.dataSource.data || []).forEach(n => this.selectedIds.add(n.id));
    }

    valorCalculado(n: NotaFiscal): number {
        if (typeof n.valor === 'number') return n.valor;
        const items = n.itens || [];
        return items.reduce((sum, i) => sum + (i.valor || 0), 0);
    }
}
