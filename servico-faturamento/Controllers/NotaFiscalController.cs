// Arquivo: servico-faturamento/Controllers/NotaFiscalController.cs

using Microsoft.AspNetCore.Mvc;
using ServicoFaturamento.Models; // Importa nossos modelos
using System.Collections.Concurrent; // "Banco de dados" em memória
using System.Net.Http.Json; // Para chamadas HTTP (GetFromJsonAsync)

namespace ServicoFaturamento.Controllers
{
    [ApiController]
    [Route("api/v1/faturamento/notas")]
    public class NotaFiscalController : ControllerBase
    {
        // --- BANCO DE DADOS EM MEMÓRIA ---
        private static readonly ConcurrentDictionary<string, NotaFiscal> _notas = new();
        // Contador estático para a numeração sequencial
        private static int _numeroSequencial = 0;

        // --- URL DO MICROSSERVIÇO DE ESTOQUE ---
        // IMPORTANTE: Este deve ser o URL onde seu 'servico-estoque' está rodando!
    // (O seu 'dotnet run' anterior mostrou o estoque rodando em http://localhost:5000)
    private const string _servicoEstoqueUrl = "http://localhost:5000";


        private readonly IHttpClientFactory _httpClientFactory;

        // Injeção de dependência do HttpClientFactory (para chamar o outro microsserviço)
        public NotaFiscalController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /**
         * GET /api/v1/faturamento/notas
         * Lista todas as notas fiscais.
         */
        [HttpGet]
        public IActionResult GetNotas()
        {
            return Ok(_notas.Values.ToList());
        }

        /**
         * POST /api/v1/faturamento/notas
         * Cria uma nova nota fiscal.
         */
        [HttpPost]
        public async Task<IActionResult> CriarNota([FromBody] CriarNotaRequest request)
        {
            if (request?.Itens == null || !request.Itens.Any())
            {
                return BadRequest("A nota deve ter pelo menos um item.");
            }

            var httpClient = _httpClientFactory.CreateClient();
            var novaNota = new NotaFiscal
            {
                // Pega o próximo número sequencial de forma segura (thread-safe)
                Numero = Interlocked.Increment(ref _numeroSequencial),
                Status = StatusNota.Aberta
            };

            foreach (var itemRequest in request.Itens)
            {
                // 1. CHAMA O SERVIÇO DE ESTOQUE para buscar dados do produto
                ProdutoDTO? produto;
                try
                {
                    produto = await httpClient.GetFromJsonAsync<ProdutoDTO>($"{_servicoEstoqueUrl}/api/v1/estoque/produtos/{itemRequest.ProdutoId}");
                }
                catch (Exception ex)
                {
                    // Se o serviço de estoque estiver offline, o GetFromJsonAsync falha
                    return StatusCode(503, new { error = "Serviço de estoque indisponível.", details = ex.Message });
                }

                if (produto == null)
                {
                    return BadRequest(new { error = $"Produto com ID '{itemRequest.ProdutoId}' não encontrado no estoque." });
                }

                // 2. Adiciona o item na nota
                novaNota.Itens.Add(new ItemNota
                {
                    ProdutoId = itemRequest.ProdutoId,
                    DescricaoProduto = produto.Descricao, // Salva a descrição (histórico)
                    Quantidade = itemRequest.Quantidade
                });
            }

            _notas[novaNota.Id] = novaNota;
            return CreatedAtAction(nameof(GetNotaPorId), new { id = novaNota.Id }, novaNota);
        }

        /**
         * GET /api/v1/faturamento/notas/{id}
         * Busca uma nota fiscal específica.
         */
        [HttpGet("{id}")]
        public IActionResult GetNotaPorId(string id)
        {
            if (_notas.TryGetValue(id, out var nota))
            {
                return Ok(nota);
            }
            return NotFound(new { error = "Nota fiscal não encontrada" });
        }


        /**
         * POST /api/v1/faturamento/notas/{id}/imprimir
         * Processa a "impressão" (fecha a nota e dá baixa no estoque).
         * Este é um REQUISITO OBRIGATÓRIO do teste.
         */
        [HttpPost("{id}/imprimir")]
        public async Task<IActionResult> ImprimirNota(string id)
        {
            if (!_notas.TryGetValue(id, out var nota))
            {
                return NotFound(new { error = "Nota fiscal não encontrada" });
            }

            // Não permitir a impressão de notas com status diferente de Aberta
            if (nota.Status != StatusNota.Aberta)
            {
                return BadRequest(new { error = "Esta nota fiscal já foi fechada/processada." });
            }

            var httpClient = _httpClientFactory.CreateClient();

            // --- INÍCIO: TRATAMENTO DE FALHAS (Requisito Obrigatório) ---
            foreach (var item in nota.Itens)
            {
                var payload = new AtualizarSaldoRequest { Quantidade = -item.Quantidade }; // Subtrai do estoque

                HttpResponseMessage response;
                try
                {
                    // 1. Tenta dar baixa no estoque
                    response = await httpClient.PutAsJsonAsync(
                        $"{_servicoEstoqueUrl}/api/v1/estoque/produtos/{item.ProdutoId}/atualizar-saldo",
                        payload);
                }
                catch (Exception ex)
                {
                    // Se o 'servico-estoque' CAIR no meio da operação.
                    // O sistema NÃO fecha a nota e avisa o usuário.
                    return StatusCode(503, new { error = "Falha ao comunicar com o serviço de estoque.", details = ex.Message });
                }

                // 2. Verifica se o estoque aceitou a baixa
                if (!response.IsSuccessStatusCode)
                {
                    // Se o estoque_serviço retornar um erro (ex: Saldo Insuficiente)
                    // O sistema NÃO fecha a nota e avisa o usuário.
                    var erroEstoque = await response.Content.ReadAsStringAsync();
                    return BadRequest(new { error = $"Não foi possível dar baixa no produto {item.DescricaoProduto}.", details = erroEstoque });
                }
            }
            // --- FIM: TRATAMENTO DE FALHAS ---

            // Se chegou até aqui, todas as baixas de estoque funcionaram.

            // 3. Atualizar o status da nota para Fechada
            nota.Status = StatusNota.Fechada;
            _notas[nota.Id] = nota; // Salva a alteração

            return Ok(nota); // Retorna a nota com o status "Fechada"
        }
    }


    // --- DTOs (Data Transfer Objects) ---
    // Classes auxiliares para as requisições

    public class CriarNotaRequest
    {
        public List<ItemRequest> Itens { get; set; } = new List<ItemRequest>();
    }

    public class ItemRequest
    {
        public string ProdutoId { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }

    // DTO para buscar o produto do outro microsserviço
    public class ProdutoDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int Saldo { get; set; }
    }

    // DTO para atualizar o saldo no outro microsserviço
    public class AtualizarSaldoRequest
    {
        public int Quantidade { get; set; }
    }
}