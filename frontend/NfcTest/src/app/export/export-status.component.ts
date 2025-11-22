import { CommonModule, Location } from '@angular/common';
import { Component, inject, AfterViewInit, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { JobsHubService } from '../core/jobs-hub.service';
import { ExportStatusHubService } from '../core/export-status-hub.service';
import { ApiService } from '../core/api.service';
import { ExportStatus, ExportJobState, ExportType } from '../core/models';
import { environment } from '../../environments/environment';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-export-status',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatListModule, MatToolbarModule, MatIconModule],
  templateUrl: './export-status.component.html'
})
export class ExportStatusComponent implements OnInit, AfterViewInit, OnDestroy {

  private route = inject(ActivatedRoute);
  private hub = inject(JobsHubService);
  private statusHub = inject(ExportStatusHubService);
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);
  private location = inject(Location);

  private destroy$ = new Subject<void>();

  jobId?: string;
  status?: ExportStatus;
  events: string[] = [];

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('jobId') || undefined;
  }

  ngAfterViewInit() {
    if (!this.jobId) return;

    this.hub.connect()
      .then(() => this.hub.join(this.jobId!));

    this.statusHub.connect()
      .then(() => this.statusHub.join(this.jobId!));

    this.hub.events$
      .pipe(takeUntil(this.destroy$))
      .subscribe(msg => {
        this.events.unshift(`[JobsHub] ${msg}`);
        this.cdr.detectChanges();
      });

    this.statusHub.events$
      .pipe(takeUntil(this.destroy$))
      .subscribe(msg => {
        this.events.unshift(`[ExportStatusHub] ${msg}`);
        this.cdr.detectChanges();
      });

    this.statusHub.exportStatus$
      .pipe(takeUntil(this.destroy$))
      .subscribe(s => {
        if (s.jobId === this.jobId) {
          this.status = s;
          this.events.unshift(this.formatStatusEvent(s));
          this.cdr.detectChanges();
        }
      });
    this.refresh();
  }

  refresh() {
    if (!this.jobId) return;
    this.api.getExport(this.jobId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({ next: (j) => {
        this.status = j;
        this.cdr.detectChanges();
      }});
  }

  stateLabel(s?: ExportJobState): string {
    if (s === undefined || s === null) return '-';
    if (typeof s === 'string') return s;
    return ['Queued', 'Started', 'Completed', 'Failed'][s] ?? `${s}`;
  }

  typeLabel(t?: ExportType): string {
    if (t === undefined || t === null) return '-';
    if (typeof t === 'string') return t;
    return ['Txt', 'Json'][t] ?? `${t}`;
  }

  downloadUrl(): string {
    if (!this.jobId) return this.status?.fileUrl || '';
    const apiUrl = `${environment.apiBaseUrl}/export/file/${this.jobId}`;
    const fileUrl = this.status?.fileUrl || '';
    if (fileUrl.startsWith('/api/')) return fileUrl;
    return apiUrl;
  }

  goBack() {
    this.location.back();
  }

  private formatStatusEvent(s: ExportStatus): string {
    const state = this.stateLabel(s.state);
    const type = this.typeLabel(s.type);
    const when = s.occurredAtUtc ? new Date(s.occurredAtUtc).toLocaleString() : '';
    let detail = `${state}`;

    if (state === 'Completed' && s.durationMs != null)
      detail += ` em ${s.durationMs}ms`;

    if (s.error)
      detail += ` (Erro: ${s.error})`;

    return when ? `${when} - ${detail}` : `${detail}`;
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();

    if (this.jobId) {
      this.hub.leave(this.jobId).catch(() => {});
      this.statusHub.leave(this.jobId).catch(() => {});
    }
  }
}
