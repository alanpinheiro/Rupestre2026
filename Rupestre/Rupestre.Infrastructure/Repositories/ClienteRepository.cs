using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Cliente>> GetAtivosAsync()
        => await _db.Clientes.OrderBy(e => e.Nome).ToListAsync();

    public async Task<PagedResult<Cliente>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = _db.Clientes.Where(e =>
            string.IsNullOrEmpty(search)
            || EF.Functions.Like(e.Nome, $"%{search}%")
            || (e.Cpf != null && EF.Functions.Like(e.Cpf, $"%{search}%"))
            || (e.Telefone1 != null && EF.Functions.Like(e.Telefone1, $"%{search}%")));

        int totalRecords    = await _db.Clientes.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("cpf",       "desc") => query.OrderByDescending(e => e.Cpf),
            ("cpf",       _)      => query.OrderBy(e => e.Cpf),
            ("telefone1", "desc") => query.OrderByDescending(e => e.Telefone1),
            ("telefone1", _)      => query.OrderBy(e => e.Telefone1),
            (_,           "desc") => query.OrderByDescending(e => e.Nome),
            _                     => query.OrderBy(e => e.Nome)
        };

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Cliente> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Cliente entity)
    {
        entity.Deletado = false;
        _db.Clientes.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Cliente entity)
    {
        _db.Clientes.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Clientes.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
