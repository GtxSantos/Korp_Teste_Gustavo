import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutoService, Produto } from '../../services/produto.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-produto-lista',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatSelectModule],
  // 3. Aponto para o template HTML do componente
  templateUrl: './produto-lista.html',
  // 4. Aponto para o arquivo SCSS do componente
  styleUrls: ['./produto-lista.scss']
})
export class ProdutoListaComponent implements OnInit, OnDestroy {
  produtos: Produto[] = [];
  novo: Produto = { codigo: '', descricao: '', saldo: 0 };

  private destroy$ = new Subject<void>();

  constructor(private service: ProdutoService, private snack: MatSnackBar) {}

  ngOnInit(): void {
    // Uso OnInit para carregar dados quando o componente é inicializado
    this.carregar();
  }

  ngOnDestroy(): void {
    // Sinalizo que o componente será destruído para cancelar subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }

  carregar() {
    this.service.listar().pipe(takeUntil(this.destroy$)).subscribe({ next: res => this.produtos = res, error: err => this.snack.open('Falha ao carregar produtos', 'Fechar', { duration: 3000 }) });
  }

  criar() {
    if (!this.novo.codigo || !this.novo.descricao) { this.snack.open('Preencha código e descrição', 'Fechar', { duration: 2000 }); return; }
    this.service.criar(this.novo).pipe(takeUntil(this.destroy$)).subscribe({ next: () => { this.novo = { codigo: '', descricao: '', saldo: 0 }; this.carregar(); this.snack.open('Produto criado', 'OK', { duration: 2000 }); }, error: () => this.snack.open('Erro ao criar produto', 'Fechar', { duration: 3000 }) });
  }

  ajustarSaldo(produto: Produto, delta: number) {
    if (!produto.id) return;
    this.service.atualizarSaldo(produto.id!, delta).pipe(takeUntil(this.destroy$)).subscribe({ next: () => { this.carregar(); this.snack.open('Saldo atualizado', 'OK', { duration: 1500 }); }, error: () => this.snack.open('Erro ao atualizar saldo', 'Fechar', { duration: 3000 }) });
  }
}