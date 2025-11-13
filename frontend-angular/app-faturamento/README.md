# AppFaturamento

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 20.3.9.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

## App Faturamento — Frontend

Aplicação frontend em Angular (standalone components) para o projeto técnico "Sistema de emissão de Notas Fiscais".

## Como rodar (passo-a-passo)

1. Certifique-se que os dois microsserviços .NET estão rodando nas portas:

- Serviço Estoque: `http://localhost:5125`
- Serviço Faturamento: `http://localhost:5126`

1. No diretório do frontend:

```powershell
cd d:\Projetos\Korp_Teste_Gustavo\frontend-angular\app-faturamento
npm install
ng serve -o
```

1. Acessar o app em `http://localhost:4200`.

## Endpoints usados

- Estoque (servico-estoque)
  - GET /api/v1/estoque/produtos
  - POST /api/v1/estoque/produtos
  - PUT /api/v1/estoque/produtos/{id}/atualizar-saldo
- Faturamento (servico-faturamento)
  - GET /api/v1/faturamento/notas
  - POST /api/v1/faturamento/notas
  - POST /api/v1/faturamento/notas/{id}/imprimir

As URLs estão configuradas em `src/app/services/*` e nos comentários dos componentes.

## Arquitetura (resumido)

- Frontend: Angular 20 (standalone components), Angular Material para UI.
- Backend: dois microsserviços .NET 8 (servico-estoque, servico-faturamento) comunicando via HTTP.

## O que foi implementado

- Cadastro e listagem de produtos (com ajuste de saldo).
- Criação de notas com múltiplos itens (seleção de produto + quantidade).
- Impressão/Processamento de nota: chama o serviço de faturamento, que faz baixa no estoque e fecha a nota.
- Tratamento de falhas no backend: o serviço de faturamento valida respostas do estoque e retorna 503/400 conforme o caso — o frontend exibe feedback via SnackBar.
- Uso de Angular Material para melhor usabilidade.

## Detalhamento técnico (para incluir no vídeo)

- Ciclos de vida Angular: os componentes são standalone e fazem carregamento inicial via construtor; posso migrar para `OnInit` (ngOnInit) se preferir demonstrar explicitamente o ciclo de vida.
- RxJS: uso básico de `Observable` via `HttpClient`, `pipe(retry(...), catchError(...))` nos serviços para retries e tratamento de erro.
- Bibliotecas: Angular Material (UI), RxJS (reactive), @angular/animations (animações para Material).
- Backend (.NET): uso de `ConcurrentDictionary` para banco em memória, `IHttpClientFactory` e tratamento explícito de exceções ao chamar outros microsserviços. Se desejar, posso adicionar exemplos de LINQ no backend (por ex.: filtros nas listagens).

## Roteiro sugerido para o vídeo de apresentação

1. Abertura rápida (30s): objetivo do projeto e arquitetura (frontend + 2 microsserviços).
1. Demonstração: cadastro de produto (30s).
1. Demonstração: criar nota com 2 itens (45s).
1. Demonstração: imprimir nota (30s) — mostrar indicador de processamento e alteração de status.
1. Demonstrar cenário de falha: parar o servico-estoque e tentar imprimir — mostrar tratamento de erro (30s).
1. Detalhamento técnico (1-2 min): ciclos de vida Angular, uso RxJS, bibliotecas, tratamento de erros no backend, uso de ConcurrentDictionary, ports/threads.
1. Conclusão (15s): próximos passos e observações.

## Observações e próximos passos

- Se quiser, eu posso:
  - adicionar testes unitários básicos para os services e components;
  - implementar concorrência/idempotência no backend (ex.: locks, tokens);
  - gravar um vídeo curto das telas (se você fornecer acesso ao microfone/terminal eu posso gerar um script para gravação).

---
_Gerado automaticamente como parte da entrega do teste técnico._
