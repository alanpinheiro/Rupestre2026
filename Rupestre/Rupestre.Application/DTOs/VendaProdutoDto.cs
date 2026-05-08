namespace Rupestre.Application.DTOs;

public class VendaProdutoDto
{
    public int Id { get; set; }
    public int Venda_Id { get; set; }
    public int Produto_Id { get; set; }
    public string NomeProduto { get; set; } = "";
    public decimal PrecoVenda { get; set; }
    public decimal SubTotal { get; set; }
    public int Quantidade { get; set; }
}
