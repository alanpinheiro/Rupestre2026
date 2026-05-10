using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendedorRepository : BaseRepository<Vendedor>, IVendedorRepository
{
    public VendedorRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Vendedor>> GetAtivosAsync()
        => await _db.Vendedores.OrderBy(e => e.Nome).ToListAsync();

    public async Task<PagedResult<Vendedor>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = _db.Vendedores.Where(e =>
            string.IsNullOrEmpty(search) || EF.Functions.Like(e.Nome, $"%{search}%"));

        int totalRecords    = await _db.Vendedores.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(e => e.Nome)
            : query.OrderBy(e => e.Nome);

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Vendedor> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Vendedor entity)
    {
        entity.Deletado = false;
        _db.Vendedores.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Vendedor entity)
    {
        _db.Vendedores.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Vendedores.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
