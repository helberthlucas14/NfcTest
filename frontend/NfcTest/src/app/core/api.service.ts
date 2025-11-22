import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateNotaRequest, NotaFiscal, PaginatedResponse, UpdateNotaRequest } from './models';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl.replace(/\/$/, '');

  listNotas(pageNumber?: number, pageSize?: number): Observable<PaginatedResponse<NotaFiscal>> {
    const params: Record<string, any> = {};
    if (pageNumber) params['pageNumber'] = pageNumber;
    if (pageSize) params['pageSize'] = pageSize;
    return this.http
      .get<{ meta: { currentPage: number; perPage: number; total: number }; data: any[] }>(`${this.baseUrl}/NotaFiscal`, { params })
      .pipe(
        map(resp => ({
          meta: resp.meta,
          data: (resp.data || []).map((d: any) => ({
            id: d.id,
            emissor: d.emissor,
            dataEmissao: d.dataEmissao,
            itens: d.items ?? d.itens ?? [],
            valor: d.valoTotal ?? d.valor,
          }))
        }))
      );
  }

   createNota(req: CreateNotaRequest): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(`${this.baseUrl}/NotaFiscal`, req);
  }

    updateNota(id: number, req: UpdateNotaRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/NotaFiscal/${id}`, req);
  }

    deleteNota(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/NotaFiscal/${id}`);
  }

}
