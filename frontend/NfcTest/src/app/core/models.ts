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