import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, retry } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

export interface Produto { id?: string; codigo: string; descricao: string; saldo: number }

@Injectable({ providedIn: 'root' })
export class ProdutoService {
  private baseUrl = 'http://localhost:5000/api/v1/estoque/produtos';
  constructor(private http: HttpClient) {}

  listar(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.baseUrl).pipe(retry(1), catchError(this.onError));
  }

  criar(produto: Produto) {
    return this.http.post<Produto>(this.baseUrl, produto).pipe(catchError(this.onError));
  }

  atualizarSaldo(id: string, quantidade: number) {
    return this.http.put(`${this.baseUrl}/${id}/atualizar-saldo`, { quantidade }).pipe(catchError(this.onError));
  }

  getById(id: string) {
    return this.http.get<Produto>(`${this.baseUrl}/${id}`).pipe(retry(1), catchError(this.onError));
  }

  private onError(err: any) {
    return throwError(() => err);
  }
}
