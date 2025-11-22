import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { ExportStatus } from './models';

@Injectable({ providedIn: 'root' })
export class ExportStatusHubService {
  private connection?: signalR.HubConnection;
  private connected = false;
  private joinedJobIds = new Set<string>();

  readonly exportStatus$ = new Subject<ExportStatus>();
  readonly events$ = new Subject<string>();

  private hubUrl(): string {
    const base = environment.apiBaseUrl;
    const host = base.replace(/\/api\/?$/, '');
    const root = host || '';
    const path = (environment as any).exportStatusHubPath || '/hubs/export-status';
    return `${root}${path}`;
  }

  async connect(): Promise<void> {
    if (this.connected && this.connection) return;
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl())
      .withAutomaticReconnect()
      .build();

    this.connection.on('ExportStatusUpdated', (payload: any) => {
      const s = this.normalizeStatus(payload);
      this.exportStatus$.next(s);
    });

    this.connection.onreconnecting(() => {
      this.events$.next('Reconectando ao ExportStatusHub...');
    });
    this.connection.onreconnected(() => {
      this.events$.next('Reconectado ao ExportStatusHub, reentrando nos grupos...');
      this.rejoinAll().catch(() => {});
    });
    this.connection.onclose(err => {
      this.events$.next(`ExportStatusHub desconectado${err ? ': ' + err.message : ''}`);
    });

    await this.connection.start();
    this.connected = true;
    this.events$.next('Conectado ao ExportStatusHub');
  }

  async join(jobId: string): Promise<void> {
    if (!this.connection) await this.connect();
    try {
      await this.connection!.invoke('JoinJobGroup', jobId);
      this.joinedJobIds.add(jobId);
      this.events$.next(`Ingressou no grupo do job ${jobId}`);
    } catch (err) {
      this.events$.next(`Falha ao ingressar no grupo do job ${jobId}`);
    }
  }

  async leave(jobId: string): Promise<void> {
    if (!this.connection) return;
    try {
      await this.connection.invoke('LeaveJobGroup', jobId);
      this.events$.next(`Saiu do grupo do job ${jobId}`);
    } catch {}
    this.joinedJobIds.delete(jobId);
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    await this.connection.stop();
    this.connected = false;
    this.joinedJobIds.clear();
  }

  private normalizeStatus(s: any): ExportStatus {
    return {
      jobId: s.jobId ?? s.JobId,
      correlationId: s.correlationId ?? s.CorrelationId,
      state: s.state ?? s.State,
      type: s.type ?? s.Type,
      ids: s.ids ?? s.Ids ?? [],
      durationMs: s.durationMs ?? s.DurationMs ?? null,
      error: s.error ?? s.Error ?? null,
      fileUrl: s.fileUrl ?? s.FileUrl ?? null,
      occurredAtUtc: s.occurredAtUtc ?? s.OccurredAtUtc,
    } as ExportStatus;
  }

  private async rejoinAll(): Promise<void> {
    if (!this.connection) return;
    for (const id of this.joinedJobIds) {
      try {
        await this.connection.invoke('JoinJobGroup', id);
        this.events$.next(`Reentrou no grupo do job ${id}`);
      } catch {}
    }
  }
}
