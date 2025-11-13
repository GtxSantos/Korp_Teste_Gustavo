import { Routes } from '@angular/router';

// 1. Importamos os novos componentes (páginas) que acabamos de criar
import { ProdutoListaComponent } from './pages/produto-lista/produto-lista';
import { NotaListaComponent } from './pages/nota-lista/nota-lista';

export const routes: Routes = [
    // 2. Definimos as rotas
    {
        path: 'produtos', // Quando a URL for /produtos
        component: ProdutoListaComponent // Carregue este componente
    },
    {
        path: 'notas', // Quando a URL for /notas
        component: NotaListaComponent // Carregue este componente
    },

    // 3. (Opcional) Rota padrão: se a URL for vazia, redireciona para /produtos
    {
        path: '',
        redirectTo: '/produtos',
        pathMatch: 'full'
    }
];