namespace Rupestre.Domain.Entities;

public class VendaProduto
{
    public int Id { get; set; }
    public int Venda_Id { get; set; }
    public int Produto_Id { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal SubTotal { get; set; }
    public int Quantidade { get; set; }
}
