namespace Rupestre.Domain.Entities;

public class Despesa
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataDespesa { get; set; }
    public decimal ValorDespesa { get; set; }
    public int StatusDespesa { get; set; }
    public bool Deletado { get; set; }
    public int Caixa_Id { get; set; }
}
