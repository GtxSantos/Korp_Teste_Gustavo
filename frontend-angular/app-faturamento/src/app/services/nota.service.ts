import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, retry } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

export interface ItemRequest { produtoId: string; quantidade: number }
export interface Nota { id?: string; numero: number; status: string; itens: any[] }

@Injectable({ providedIn: 'root' })
export class NotaService {
  private baseUrl = 'http://localhost:5126/api/v1/faturamento/notas';
  constructor(private http: HttpClient) {}

  listar(): Observable<Nota[]> {
    return this.http.get<Nota[]>(this.baseUrl).pipe(retry(1), catchError(this.onError));
  }

  criar(itens: ItemRequest[]) {
    return this.http.post<Nota>(this.baseUrl, { itens }).pipe(catchError(this.onError));
  }

  imprimir(id?: string) {
    if (!id) return this.http.post(this.baseUrl, {} as any); // fallback
    return this.http.post(`${this.baseUrl}/${id}/imprimir`, {}).pipe(catchError(this.onError));
  }

  private onError(err: any) {
    return throwError(() => err);
  }
}
