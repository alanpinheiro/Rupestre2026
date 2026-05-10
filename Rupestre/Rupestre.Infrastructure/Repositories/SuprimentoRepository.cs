using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class SuprimentoRepository : BaseRepository<Suprimento>, ISuprimentoRepository
{
    public SuprimentoRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Suprimento>> GetByCaixaAsync(int caixaId)
        => await _db.Suprimentos
            .Where(e => e.Caixa_Id == caixaId)
            .OrderByDescending(e => e.Id)
            .ToListAsync();

    public async Task<PagedResult<Suprimento>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var query = _db.Suprimentos;

        int totalRecords    = await query.CountAsync();
        int filteredRecords = totalRecords;

        IQueryable<Suprimento> ordered = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("valorsuprimento", "desc") => query.OrderByDescending(e => e.ValorSuprimento),
            ("valorsuprimento", _)      => query.OrderBy(e => e.ValorSuprimento),
            (_,                 "desc") => query.OrderByDescending(e => e.Id),
            _                           => query.OrderBy(e => e.Id)
        };

        var data = await ordered.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Suprimento> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Suprimento entity)
    {
        entity.Deletado = false;
        _db.Suprimentos.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Suprimento entity)
    {
        _db.Suprimentos.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Suprimentos.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
