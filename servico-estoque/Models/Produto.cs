// Arquivo: servico-estoque/Models/Produto.cs

namespace ServicoEstoque.Models
{
    // Modelo que representa um produto no meu serviço de estoque.
    // Mantive somente os campos necessários para o teste e para geração do PDF.
    public class Produto
    {
        // Id interno gerado automaticamente
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        // Código do produto (campo exigido no PDF)
        public string Codigo { get; set; } = string.Empty;

        // Descrição do produto (campo exigido no PDF)
        public string Descricao { get; set; } = string.Empty;

        // Saldo disponível em estoque
        public int Saldo { get; set; }
    }
}