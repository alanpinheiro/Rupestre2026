namespace Rupestre.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardData> GetDashboardDataAsync();
}

public class DashboardData
{
    public decimal VendasHoje { get; set; }
    public int MoveisAEntregar { get; set; }
    public int MontagensParaHoje { get; set; }
    public IEnumerable<MesFaturamento> Faturamento { get; set; } = [];
    public IEnumerable<AbcItem> Abc { get; set; } = [];
    public IEnumerable<StatusEntregaItem> StatusEntregas { get; set; } = [];
}

public class MesFaturamento
{
    public string Mes { get; set; } = string.Empty;
    public decimal Faturamento { get; set; }
    public decimal Recebimento { get; set; }
}

public class AbcItem
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeVendida { get; set; }
    public int Estoque { get; set; }
}

public class StatusEntregaItem
{
    public int StatusEntrega { get; set; }
    public int Qty { get; set; }
}
