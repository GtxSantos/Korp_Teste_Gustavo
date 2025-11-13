import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotaService, ItemRequest, Nota } from '../../services/nota.service';
import { ProdutoService, Produto } from '../../services/produto.service';
import { forkJoin, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';

interface ItemForm { produtoId: string; quantidade: number }

@Component({
  selector: 'app-nota-lista',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatSelectModule],
  // 5. Aponto para o template HTML do componente
  templateUrl: './nota-lista.html',
  // 6. Aponto para o arquivo SCSS do componente
  styleUrls: ['./nota-lista.scss']
})
export class NotaListaComponent implements OnInit, OnDestroy {
  notas: Nota[] = [];
  produtos: Produto[] = [];

  // Formulário simples para criar uma nova nota
  itensForm: ItemForm[] = [{ produtoId: '', quantidade: 1 }];

  loading = false;
  error: string | null = null;

  private destroy$ = new Subject<void>();

  constructor(private service: NotaService, private produtoService: ProdutoService, private snack: MatSnackBar) {}

  ngOnInit(): void {
    this.carregar();
    this.carregarProdutos();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  carregar() {
    this.service.listar().pipe(takeUntil(this.destroy$)).subscribe({ next: r => this.notas = r, error: () => this.snack.open('Falha ao carregar notas', 'Fechar', { duration: 3000 }) });
  }

  carregarProdutos() {
    this.produtoService.listar().pipe(takeUntil(this.destroy$)).subscribe({ next: r => this.produtos = r, error: () => this.snack.open('Falha ao carregar produtos', 'Fechar', { duration: 3000 }) });
  }

  adicionarLinha() {
    this.itensForm.push({ produtoId: '', quantidade: 1 });
  }

  removerLinha(i: number) {
    this.itensForm.splice(i, 1);
  }

  criarNota() {
    const itens: ItemRequest[] = this.itensForm.map(i => ({ produtoId: i.produtoId, quantidade: i.quantidade }));
    this.loading = true; this.error = null;
    this.service.criar(itens).subscribe({ next: () => { this.loading = false; this.itensForm = [{ produtoId: '', quantidade: 1 }]; this.carregar(); this.carregarProdutos(); this.snack.open('Nota criada', 'OK', { duration: 2000 }); }, error: (err) => { this.loading = false; this.snack.open('Erro ao criar nota: ' + JSON.stringify(err?.error || err), 'Fechar', { duration: 5000 }); } });
  }

  imprimir(nota: Nota) {
    if (nota.status !== 'Aberta') { this.snack.open('Nota não está aberta', 'OK', { duration: 1500 }); return; }
    this.loading = true; this.error = null;

  // Faço uma checagem prévia de saldos para evitar erro de "Saldo insuficiente" ao imprimir
    const checks = nota.itens.map(i => this.produtoService.getById(i.produtoId));
    forkJoin(checks).subscribe({
      next: prods => {
  // Verifico se algum produto não possui saldo suficiente
        for (let idx = 0; idx < prods.length; idx++) {
          const produto = prods[idx];
          const quantidade = nota.itens[idx].quantidade;
          if (!produto || produto.saldo < quantidade) {
            this.loading = false;
            this.snack.open(`Não é possível imprimir: saldo insuficiente para '${produto?.descricao || nota.itens[idx].produtoId}' (necessário ${quantidade}, disponível ${produto?.saldo ?? 0})`, 'Fechar', { duration: 6000 });
            return;
          }
        }

  // Se todos os saldos estiverem suficientes, prossigo com a impressão
        this.service.imprimir(nota.id).subscribe({ next: () => { this.loading = false; this.carregar(); this.carregarProdutos(); this.snack.open('Impressão concluída. Nota fechada.', 'OK', { duration: 2500 }); }, error: (err) => { this.loading = false; const detail = err?.error?.details || err?.error || err; this.snack.open('Erro ao imprimir: ' + JSON.stringify(detail), 'Fechar', { duration: 6000 }); } });
      },
      error: (err) => { this.loading = false; this.snack.open('Falha ao verificar saldos: ' + JSON.stringify(err?.error || err), 'Fechar', { duration: 5000 }); }
    });
  }
}