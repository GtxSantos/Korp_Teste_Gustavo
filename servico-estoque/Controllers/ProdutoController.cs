
// Arquivo: servico-estoque/Controllers/ProdutoController.cs

using Microsoft.AspNetCore.Mvc;
using ServicoEstoque.Models; // Importo o modelo Produto usado por este serviço
using System.Collections.Concurrent; // Uso um dicionário concorrente como armazenamento em memória

namespace ServicoEstoque.Controllers
{
    [ApiController]
    // Defino o prefixo da URL para os endpoints deste controlador
    [Route("api/v1/estoque/produtos")]
    public class ProdutoController : ControllerBase
    {
    // --- ARMAZENAMENTO EM MEMÓRIA ---
    // Para este teste mantenho um dicionário estático em memória.
    // Uso 'ConcurrentDictionary' para segurança em cenários multithread.
        private static readonly ConcurrentDictionary<string, Produto> _produtos = new();

    // --- ENDPOINTS DA API ---

    /**
     * POST /api/v1/estoque/produtos
     * Crio um novo produto e retorno 201 com a entidade criada.
     */
        [HttpPost]
        public IActionResult CriarProduto([FromBody] Produto novoProduto)
        {
            if (novoProduto == null)
            {
                return BadRequest("Dados do produto inválidos.");
            }

            // Gero um ID único
            novoProduto.Id = Guid.NewGuid().ToString("N");
            _produtos[novoProduto.Id] = novoProduto;
            // Retorno 201 Created com o produto criado
            return CreatedAtAction(nameof(GetProdutoPorId), new { id = novoProduto.Id }, novoProduto);
        }

    /**
     * GET /api/v1/estoque/produtos
     * Retorno a lista de produtos armazenados.
     */
        [HttpGet]
        public IActionResult GetProdutos()
        {
            // Retorno todos os produtos do armazenamento em memória
            return Ok(_produtos.Values.ToList());
        }

    /**
     * GET /api/v1/estoque/produtos/{id}
     * Busco um produto pelo Id e retorno 200 ou 404.
     */
        [HttpGet("{id}")]
        public IActionResult GetProdutoPorId(string id)
        {
            if (_produtos.TryGetValue(id, out var produto))
            {
                // Produto encontrado — retorno 200 OK
                return Ok(produto);
            }
            // Produto não encontrado — retorno 404
            return NotFound(new { error = "Produto não encontrado" });
        }

    /**
     * PUT /api/v1/estoque/produtos/{id}/atualizar-saldo
     * Atualizo o saldo de um produto (chamado pelo serviço de faturamento).
     */
        [HttpPut("{id}/atualizar-saldo")]
        public IActionResult AtualizarSaldo(string id, [FromBody] AtualizarSaldoRequest request)
        {
            if (!_produtos.TryGetValue(id, out var produto))
            {
                return NotFound(new { error = "Produto não encontrado" });
            }

            // Calculo o novo saldo
            var novoSaldo = produto.Saldo + request.Quantidade;

            // Validação: não permito saldo negativo
            if (novoSaldo < 0)
            {
                return BadRequest(new { error = "Saldo insuficiente" });
            }

            // Atualizo e salvo o novo saldo
            produto.Saldo = novoSaldo;
            _produtos[id] = produto;

            return Ok(produto);
        }
    }

    // Classe DTO (Data Transfer Object) auxiliar
    // Usada apenas para receber o corpo da requisição de AtualizarSaldo
    public class AtualizarSaldoRequest
    {
        // A quantidade a ser adicionada/subtraída.
        // Ex: -2 para subtrair 2 do estoque.
        public int Quantidade { get; set; }
    }
}