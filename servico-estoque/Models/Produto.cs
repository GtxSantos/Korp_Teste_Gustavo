// Localização: servico-estoque/Models/Produto.cs

namespace ServicoEstoque.Models
{
    // Este é o "molde" para os dados do produto,
    // conforme os campos obrigatórios do PDF.
    public class Produto
    {
        // Usamos um Id interno para facilitar o gerenciamento no banco
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        // "Código" (requisito do PDF)
        public string Codigo { get; set; }

        // "Descrição" (requisito do PDF)
        public string Descricao { get; set; }

        // "Saldo" (requisito do PDF)
        public int Saldo { get; set; }
    }
}