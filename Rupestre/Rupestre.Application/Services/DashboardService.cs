using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardDto> GetAsync()
    {
        var data = await _repo.GetDashboardDataAsync();

        var fat = data.Faturamento.ToList();
        var abc = data.Abc.ToList();

        return new DashboardDto
        {
            VendasHoje = data.VendasHoje,
            MoveisAEntregar = data.MoveisAEntregar,
            MontagensParaHoje = data.MontagensParaHoje,

            FaturamentoLabels = fat.Select(f => f.Mes).ToList(),
            FaturamentoSeries = fat.Select(f => f.Faturamento).ToList(),
            RecebimentoSeries = fat.Select(f => f.Recebimento).ToList(),

            AbcLabels = abc.Select(a => a.Nome).ToList(),
            AbcSeries = abc.Select(a => a.QuantidadeVendida).ToList(),
            AbcEstoques = abc.Select(a => a.Estoque).ToList()
        };
    }
}
