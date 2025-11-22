export interface NotaItem {
  id?: number;
  notaFiscalId?: number;
  descricao: string;
  valor: number;
}

export interface NotaFiscal {
  id: number;
  emissor: string;
  dataEmissao: string;
  itens?: NotaItem[];
  valor?: number;
}

export interface CreateNotaRequest {
  emissor: string;
  dataEmissao: string;
  items?: Array<{ descricao: string; valor: number }>;
}

export interface UpdateNotaRequest {
  emissor: string;
  dataEmissao: string;
  items?: Array<{ id?: number; descricao: string; valor: number }>;
}

export interface ApiMeta {
  currentPage: number;
  perPage: number;
  total: number;
}

export interface PaginatedResponse<T> {
  meta: ApiMeta;
  data: T[];
}

export type JobStatus = 'Pending' | 'InProgress' | 'Completed' | 'Error';

export interface JobRecord {
  jobId: string;
  status: JobStatus;
  createdAt: string;
  updatedAt: string;
  lastMessage?: string;
  correlationId?: string;
  format?: string;
  noteIds?: number[];
}

export interface ExportStartResponse { jobId: string; correlationId: string }

export type ExportJobState = 'Queued' | 'Started' | 'Completed' | 'Failed' | number;
export type ExportType = 'Txt' |  'Json' | number;

export interface ExportStatus {
  jobId: string;
  correlationId: string;
  state: ExportJobState;
  type: ExportType;
  ids: number[];
  durationMs?: number | null;
  error?: string | null;
  fileUrl?: string | null;
  occurredAtUtc: string;
}

