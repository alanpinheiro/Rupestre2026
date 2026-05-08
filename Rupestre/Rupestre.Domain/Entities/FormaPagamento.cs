namespace Rupestre.Domain.Entities;

public class FormaPagamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal? TaxaAdministrativa { get; set; }
    public decimal? Fator1 { get; set; }
    public decimal? Fator2 { get; set; }
    public decimal? Fator3 { get; set; }
    public decimal? Fator4 { get; set; }
    public decimal? Fator5 { get; set; }
    public decimal? Fator6 { get; set; }
    public decimal? Fator7 { get; set; }
    public decimal? Fator8 { get; set; }
    public decimal? Fator9 { get; set; }
    public decimal? Fator10 { get; set; }
    public decimal? Fator11 { get; set; }
    public decimal? Fator12 { get; set; }
    public bool Deletado { get; set; }
}
