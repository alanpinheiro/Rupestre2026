namespace Rupestre.Domain.Entities;

public class VendaPagamento
{
    public int Id { get; set; }
    public int Venda_Id { get; set; }
    public int FormaPagamento_Id { get; set; }
    public int TipoPagamento_Id { get; set; }
    public int Parcelas { get; set; }
    public decimal ValorPagamento { get; set; }
    public decimal ValorLiquidoPagamento { get; set; }
    public int? Caixa_Id { get; set; }
}
