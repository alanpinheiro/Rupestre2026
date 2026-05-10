using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class FormaPagamentoRepository : BaseRepository<FormaPagamento>, IFormaPagamentoRepository
{
    public FormaPagamentoRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<FormaPagamento>> GetAtivosAsync()
        => await _db.FormasPagamento.OrderBy(e => e.Nome).ToListAsync();

    public async Task<PagedResult<FormaPagamento>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = _db.FormasPagamento.Where(e =>
            string.IsNullOrEmpty(search) || EF.Functions.Like(e.Nome, $"%{search}%"));

        int totalRecords    = await _db.FormasPagamento.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("taxaadministrativa", "desc") => query.OrderByDescending(e => e.TaxaAdministrativa),
            ("taxaadministrativa", _)      => query.OrderBy(e => e.TaxaAdministrativa),
            (_,                    "desc") => query.OrderByDescending(e => e.Nome),
            _                              => query.OrderBy(e => e.Nome)
        };

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<FormaPagamento> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(FormaPagamento entity)
    {
        entity.Deletado = false;
        _db.FormasPagamento.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(FormaPagamento entity)
    {
        _db.FormasPagamento.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.FormasPagamento.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
