namespace Rupestre.Domain.Entities;

public class VendaRemessa
{
    public int Id { get; set; }
    public decimal ValorRemessa { get; set; }
    public int Caixa_Id { get; set; }
    public bool? Deletado { get; set; }
}
