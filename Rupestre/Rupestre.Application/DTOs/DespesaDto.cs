using Rupestre.Domain.Enums;

namespace Rupestre.Application.DTOs;

public class DespesaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataDespesa { get; set; }
    public decimal ValorDespesa { get; set; }
    public StatusDespesa StatusDespesa { get; set; }
    public int Caixa_Id { get; set; }
}
