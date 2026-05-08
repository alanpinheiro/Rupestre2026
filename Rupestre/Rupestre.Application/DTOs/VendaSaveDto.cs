using Rupestre.Domain.Enums;

namespace Rupestre.Application.DTOs;

public class VendaSaveDto
{
    public int Id { get; set; }
    public int? NumeroPedidoExterno { get; set; }
    public DateTime DataVenda { get; set; }
    public StatusVenda StatusVenda { get; set; } = StatusVenda.Pendente;
    public DateTime? DataEntrega { get; set; }
    public StatusEntrega StatusEntrega { get; set; } = StatusEntrega.Agendada;
    public string? Observacao { get; set; }
    public decimal? Frete { get; set; }
    public decimal? ResidenciaPagamento { get; set; }
    public int Cliente_Id { get; set; }
    public int Vendedor_Id { get; set; }
    public List<VendaProdutoDto> Produtos { get; set; } = [];
    public List<VendaPagamentoDto> Pagamentos { get; set; } = [];
}
