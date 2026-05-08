namespace Rupestre.Application.DTOs;

public class DashboardDto
{
    public decimal VendasHoje { get; set; }
    public int MoveisAEntregar { get; set; }
    public int MontagensParaHoje { get; set; }

    public List<string> FaturamentoLabels { get; set; } = [];
    public List<decimal> FaturamentoSeries { get; set; } = [];
    public List<decimal> RecebimentoSeries { get; set; } = [];

    public List<string> AbcLabels { get; set; } = [];
    public List<int> AbcSeries { get; set; } = [];
    public List<int> AbcEstoques { get; set; } = [];

    public int EntregasEntregues { get; set; }
    public int EntregasAgendadas { get; set; }
    public int EntregasAtrasadas { get; set; }
}
