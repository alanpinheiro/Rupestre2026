namespace Rupestre.Domain.Entities;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int? MinEstoque { get; set; }
    public int? MaxEstoque { get; set; }
    public int Estoque { get; set; }
    public decimal PrecoCusto { get; set; }
    public decimal PrecoVenda { get; set; }
    public int Fabricante_Id { get; set; }
    public bool Deletado { get; set; }
}
