namespace Rupestre.Domain.Entities;

public class Caixa
{
    public int Id { get; set; }
    public DateTime DataAbertura { get; set; }
    public bool OnOff { get; set; }
    public decimal ValorAbertura { get; set; }
    public decimal? DinheiroNoCaixa { get; set; }
}
