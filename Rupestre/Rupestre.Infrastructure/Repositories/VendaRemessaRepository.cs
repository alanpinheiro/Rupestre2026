using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaRemessaRepository : BaseRepository<VendaRemessa>, IVendaRemessaRepository
{
    public VendaRemessaRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<VendaRemessa>> GetByCaixaAsync(int caixaId)
        => await _db.VendaRemessas
            .Where(e => e.Caixa_Id == caixaId)
            .OrderByDescending(e => e.Id)
            .ToListAsync();

    public async Task<PagedResult<VendaRemessa>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var query = _db.VendaRemessas;

        int totalRecords    = await query.CountAsync();
        int filteredRecords = totalRecords;

        IQueryable<VendaRemessa> ordered = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("valorremessa", "desc") => query.OrderByDescending(e => e.ValorRemessa),
            ("valorremessa", _)      => query.OrderBy(e => e.ValorRemessa),
            ("caixa_id",     "desc") => query.OrderByDescending(e => e.Caixa_Id),
            ("caixa_id",     _)      => query.OrderBy(e => e.Caixa_Id),
            (_,              "desc") => query.OrderByDescending(e => e.Id),
            _                        => query.OrderBy(e => e.Id)
        };

        var data = await ordered.Skip(start).Take(length).ToListAsync();

        return new PagedResult<VendaRemessa> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(VendaRemessa entity)
    {
        entity.Deletado = false;
        _db.VendaRemessas.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(VendaRemessa entity)
    {
        _db.VendaRemessas.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.VendaRemessas.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
