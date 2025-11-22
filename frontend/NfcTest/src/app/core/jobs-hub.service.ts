import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class JobsHubService {
  private connection?: signalR.HubConnection;
  private connected = false;
  private joinedJobIds = new Set<string>();

  readonly events$ = new Subject<string>();
  readonly jobStarted$ = new Subject<{ jobId: string }>();
  readonly jobProgress$ = new Subject<{ jobId: string; message: string }>();
  readonly jobCompleted$ = new Subject<{ jobId: string; durationMs: number }>();
  readonly jobError$ = new Subject<{ jobId: string; error: string }>();

  private hubUrl(): string {
    const base = environment.apiBaseUrl;
    const host = base.replace(/\/api\/?$/, '');
    const root = host || '';
    const path = (environment as any).jobsHubPath || '/hubs/jobs';
    return `${root}${path}`;
  }

  async connect(): Promise<void> {
    if (this.connected && this.connection) return;
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl())
      .withAutomaticReconnect()
      .build();

    this.connection.on('jobStarted', (payload: any) => {
      this.jobStarted$.next({ jobId: payload.jobId });
      this.events$.next(`jobStarted: ${payload.jobId}`);
    });
    this.connection.on('jobProgress', (payload: any) => {
      this.jobProgress$.next({ jobId: payload.jobId, message: payload.message });
      this.events$.next(`jobProgress: ${payload.message}`);
    });
    this.connection.on('jobCompleted', (payload: any) => {
      this.jobCompleted$.next({ jobId: payload.jobId, durationMs: payload.durationMs });
      this.events$.next(`jobCompleted em ${payload.durationMs}ms`);
    });
    this.connection.on('jobError', (payload: any) => {
      this.jobError$.next({ jobId: payload.jobId, error: payload.error });
      this.events$.next(`jobError: ${payload.error}`);
    });

    this.connection.onreconnecting(() => {
      this.events$.next('Reconectando ao JobsHub...');
    });
    this.connection.onreconnected(() => {
      this.events$.next('Reconectado ao JobsHub, reentrando nos grupos...');
      this.rejoinAll().catch(() => {});
    });
    this.connection.onclose(err => {
      this.events$.next(`JobsHub desconectado${err ? ': ' + err.message : ''}`);
    });

    await this.connection.start();
    this.connected = true;
    this.events$.next('Conectado ao JobsHub');
  }

  async join(jobId: string): Promise<void> {
    if (!this.connection) await this.connect();
    await this.connection!.invoke('JoinJobGroup', jobId);
    this.joinedJobIds.add(jobId);
    this.events$.next(`Ingressou no grupo do job ${jobId}`);
  }

  async leave(jobId: string): Promise<void> {
    if (!this.connection) return;
    await this.connection.invoke('LeaveJobGroup', jobId);
    this.joinedJobIds.delete(jobId);
    this.events$.next(`Saiu do grupo do job ${jobId}`);
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    await this.connection.stop();
    this.connected = false;
    this.joinedJobIds.clear();
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