namespace Rupestre.Application.DTOs;

public class VendaPagamentoDto
{
    public int Id { get; set; }
    public int Venda_Id { get; set; }
    public int FormaPagamento_Id { get; set; }
    public string NomeFormaPagamento { get; set; } = "";
    public int TipoPagamento_Id { get; set; }
    public int Parcelas { get; set; }
    public decimal ValorPagamento { get; set; }
    public decimal ValorLiquidoPagamento { get; set; }
}
