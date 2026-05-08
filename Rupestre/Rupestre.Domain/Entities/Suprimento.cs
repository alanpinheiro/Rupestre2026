namespace Rupestre.Domain.Entities;

public class Suprimento
{
    public int Id { get; set; }
    public int Caixa_Id { get; set; }
    public decimal ValorSuprimento { get; set; }
    public bool Deletado { get; set; }
}
