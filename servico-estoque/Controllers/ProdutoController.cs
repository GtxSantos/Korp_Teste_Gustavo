// Localização: servico-estoque/Controllers/ProdutoController.cs

using Microsoft.AspNetCore.Mvc;
using ServicoEstoque.Models; // Importa o "molde" que acabamos de criar
using System.Collections.Concurrent; // Para um "banco de dados" em memória seguro

namespace ServicoEstoque.Controllers
{
    [ApiController]
    // Este é o prefixo da URL para todos os endpoints deste controlador
    // Conforme definimos no 'api_contratos.md'
    [Route("api/v1/estoque/produtos")]
    public class ProdutoController : ControllerBase
    {
        // --- BANCO DE DADOS EM MEMÓRIA ---
        // Para este teste, um "banco de dados" em memória (um dicionário estático)
        // é a forma mais rápida e eficiente de implementar o backend.
        // Usamos 'ConcurrentDictionary' para segurança em ambientes com múltiplas threads.
        private static readonly ConcurrentDictionary<string, Produto> _produtos = new();

        // --- ENDPOINTS DA API ---

        /**
         * POST /api/v1/estoque/produtos
         * Cria um novo produto.
         */
        [HttpPost]
        public IActionResult CriarProduto([FromBody] Produto novoProduto)
        {
            if (novoProduto == null)
            {
                return BadRequest("Dados do produto inválidos.");
            }

            // Define um ID único
            novoProduto.Id = Guid.NewGuid().ToString("N");
            _produtos[novoProduto.Id] = novoProduto;

            // Retorna 201 Created e o produto criado
            return CreatedAtAction(nameof(GetProdutoPorId), new { id = novoProduto.Id }, novoProduto);
        }

        /**
         * GET /api/v1/estoque/produtos
         * Lista todos os produtos.
         */
        [HttpGet]
        public IActionResult GetProdutos()
        {
            // Retorna a lista de todos os valores do nosso "banco"
            return Ok(_produtos.Values.ToList());
        }

        /**
         * GET /api/v1/estoque/produtos/{id}
         * Busca um produto específico pelo seu Id.
         */
        [HttpGet("{id}")]
        public IActionResult GetProdutoPorId(string id)
        {
            if (_produtos.TryGetValue(id, out var produto))
            {
                // Encontrou o produto, retorna 200 OK
                return Ok(produto);
            }
            // Não encontrou, retorna 404 Not Found
            return NotFound(new { error = "Produto não encontrado" });
        }

        /**
         * PUT /api/v1/estoque/produtos/{id}/atualizar-saldo
         * Atualiza o saldo de um produto (usado pelo Serviço de Faturamento).
         */
        [HttpPut("{id}/atualizar-saldo")]
        public IActionResult AtualizarSaldo(string id, [FromBody] AtualizarSaldoRequest request)
        {
            if (!_produtos.TryGetValue(id, out var produto))
            {
                return NotFound(new { error = "Produto não encontrado" });
            }

            // Calcula o novo saldo
            var novoSaldo = produto.Saldo + request.Quantidade;

            // Validação de saldo (não pode ficar negativo)
            if (novoSaldo < 0)
            {
                return BadRequest(new { error = "Saldo insuficiente" });
            }

            // Atualiza o saldo
            produto.Saldo = novoSaldo;
            _produtos[id] = produto; // Salva de volta no "banco"

            return Ok(produto); // Retorna o produto com o saldo atualizado
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