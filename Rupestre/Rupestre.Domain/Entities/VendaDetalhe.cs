using Rupestre.Domain.Enums;

namespace Rupestre.Domain.Entities;

public class VendaDetalhe
{
    public int Id { get; set; }
    public int? NumeroPedidoExterno { get; set; }
    public DateTime DataVenda { get; set; }
    public StatusVenda StatusVenda { get; set; }
    public DateTime? DataEntrega { get; set; }
    public StatusEntrega StatusEntrega { get; set; }
    public string? Observacao { get; set; }
    public decimal? Frete { get; set; }
    public decimal? ResidenciaPagamento { get; set; }
    public int Caixa_Id { get; set; }
    public int Cliente_Id { get; set; }
    public int Vendedor_Id { get; set; }
    public string NomeCliente { get; set; } = "";
    public string NomeVendedor { get; set; } = "";
    public decimal TotalVenda { get; set; }
}
