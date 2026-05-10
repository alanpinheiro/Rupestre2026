using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Enums;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _db;

    public DashboardRepository(ApplicationDbContext db) => _db = db;

    public async Task<DashboardData> GetDashboardDataAsync()
    {
        var today     = DateTime.Today;
        var startDate = new DateTime(today.AddMonths(-5).Year, today.AddMonths(-5).Month, 1);

        var vendasHoje = await _db.VendaProdutos
            .Where(vp => _db.Vendas.Any(v =>
                v.Id == vp.Venda_Id
                && v.DataVenda.Date == today
                && v.StatusVenda != StatusVenda.Cancelada))
            .SumAsync(vp => (decimal?)vp.SubTotal) ?? 0m;

        var moveisAEntregar = await _db.Vendas.CountAsync(v =>
            (v.StatusEntrega == StatusEntrega.Agendada || v.StatusEntrega == StatusEntrega.Atrasada)
            && v.StatusVenda != StatusVenda.Cancelada);

        var montagensParaHoje = await _db.Vendas.CountAsync(v =>
            v.DataEntrega.HasValue
            && v.DataEntrega.Value.Date == today
            && v.StatusEntrega != StatusEntrega.Entregue
            && v.StatusVenda != StatusVenda.Cancelada);

        var fatRaw = await (from vp in _db.VendaProdutos
                            join v in _db.Vendas on vp.Venda_Id equals v.Id
                            where v.DataVenda >= startDate && v.StatusVenda != StatusVenda.Cancelada
                            group vp.SubTotal by new { v.DataVenda.Year, v.DataVenda.Month } into g
                            select new { g.Key.Year, g.Key.Month, Faturamento = g.Sum() })
                           .ToListAsync();

        var recRaw = await (from pg in _db.VendaPagamentos
                            join v in _db.Vendas on pg.Venda_Id equals v.Id
                            where v.DataVenda >= startDate && v.StatusVenda != StatusVenda.Cancelada
                            group pg.ValorLiquidoPagamento by new { v.DataVenda.Year, v.DataVenda.Month } into g
                            select new { g.Key.Year, g.Key.Month, Recebimento = g.Sum() })
                           .ToListAsync();

        var months = Enumerable.Range(0, 6)
            .Select(i => today.AddMonths(-i))
            .Select(d => new { d.Year, d.Month, Label = d.ToString("MM/yyyy") })
            .OrderBy(m => m.Year * 100 + m.Month)
            .ToList();

        var faturamento = months.Select(m => new MesFaturamento
        {
            Mes         = m.Label,
            Faturamento = fatRaw.FirstOrDefault(f => f.Year == m.Year && f.Month == m.Month)?.Faturamento ?? 0m,
            Recebimento = recRaw.FirstOrDefault(r => r.Year == m.Year && r.Month == m.Month)?.Recebimento ?? 0m
        }).ToList();

        var abc = await (from vp in _db.VendaProdutos
                         join v in _db.Vendas on vp.Venda_Id equals v.Id
                         where v.StatusVenda != StatusVenda.Cancelada
                         group vp by vp.Produto_Id into g
                         orderby g.Sum(x => x.Quantidade) descending
                         select new { Produto_Id = g.Key, QuantidadeVendida = g.Sum(x => x.Quantidade) })
                        .Take(10)
                        .Join(_db.Produtos.IgnoreQueryFilters(),
                              x => x.Produto_Id,
                              p => p.Id,
                              (x, p) => new AbcItem { Nome = p.Nome, QuantidadeVendida = x.QuantidadeVendida, Estoque = p.Estoque })
                        .ToListAsync();

        var statusEntregas = await _db.Vendas
            .Where(v => v.StatusVenda != StatusVenda.Cancelada)
            .GroupBy(v => v.StatusEntrega)
            .Select(g => new StatusEntregaItem { StatusEntrega = (int)g.Key, Qty = g.Count() })
            .ToListAsync();

        return new DashboardData
        {
            VendasHoje        = vendasHoje,
            MoveisAEntregar   = moveisAEntregar,
            MontagensParaHoje = montagensParaHoje,
            Faturamento       = faturamento,
            Abc               = abc,
            StatusEntregas    = statusEntregas
        };
    }
}
