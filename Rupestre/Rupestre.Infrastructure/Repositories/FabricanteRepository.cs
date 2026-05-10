using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class FabricanteRepository : BaseRepository<Fabricante>, IFabricanteRepository
{
    public FabricanteRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Fabricante>> GetAtivosAsync()
        => await _db.Fabricantes.OrderBy(e => e.Nome).ToListAsync();

    public async Task<PagedResult<Fabricante>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = _db.Fabricantes.Where(e =>
            string.IsNullOrEmpty(search) || EF.Functions.Like(e.Nome, $"%{search}%"));

        int totalRecords    = await _db.Fabricantes.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(e => e.Nome)
            : query.OrderBy(e => e.Nome);

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Fabricante> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Fabricante entity)
    {
        entity.Deletado = false;
        _db.Fabricantes.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Fabricante entity)
    {
        _db.Fabricantes.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Fabricantes.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
