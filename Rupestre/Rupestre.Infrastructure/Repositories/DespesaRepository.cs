using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class DespesaRepository : BaseRepository<Despesa>, IDespesaRepository
{
    public DespesaRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Despesa>> GetByCaixaAsync(int caixaId)
        => await _db.Despesas
            .Where(e => e.Caixa_Id == caixaId)
            .OrderByDescending(e => e.DataDespesa)
            .ToListAsync();

    public async Task<PagedResult<Despesa>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = _db.Despesas.Where(e =>
            string.IsNullOrEmpty(search) || EF.Functions.Like(e.Nome, $"%{search}%"));

        int totalRecords    = await _db.Despesas.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("nome",          "desc") => query.OrderByDescending(e => e.Nome),
            ("nome",          _)      => query.OrderBy(e => e.Nome),
            ("valordespesa",  "desc") => query.OrderByDescending(e => e.ValorDespesa),
            ("valordespesa",  _)      => query.OrderBy(e => e.ValorDespesa),
            ("statusdespesa", "desc") => query.OrderByDescending(e => e.StatusDespesa),
            ("statusdespesa", _)      => query.OrderBy(e => e.StatusDespesa),
            (_,               "desc") => query.OrderByDescending(e => e.DataDespesa),
            _                         => query.OrderBy(e => e.DataDespesa)
        };

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Despesa> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Despesa entity)
    {
        entity.Deletado = false;
        _db.Despesas.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Despesa entity)
    {
        _db.Despesas.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Despesas.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
