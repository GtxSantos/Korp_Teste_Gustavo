// Localização: servico-faturamento/Models/NotaFiscal.cs

using System.Text.Json.Serialization;

namespace ServicoFaturamento.Models
{
    // Enum para o Status (converte para string "Aberta" ou "Fechada" no JSON)
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusNota
    {
        Aberta,
        Fechada
    }

    // O "Item" dentro da Nota Fiscal
    public class ItemNota
    {
        // ID/Código do produto (do Serviço de Estoque)
        public string ProdutoId { get; set; } = string.Empty;

        // Descrição do produto (armazenada para histórico)
        public string DescricaoProduto { get; set; } = string.Empty;

        // Quantidade usada na nota
        public int Quantidade { get; set; }
    }

    // A "Nota Fiscal" principal
    public class NotaFiscal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        // Numeração sequencial (requisito do PDF)
        public int Numero { get; set; }

        // Status: Aberta ou Fechada (requisito do PDF)
        public StatusNota Status { get; set; } = StatusNota.Aberta;

        public DateTime DataEmissao { get; set; } = DateTime.UtcNow;

        // Inclusão de múltiplos produtos (requisito do PDF)
        public List<ItemNota> Itens { get; set; } = new List<ItemNota>();
    }
}