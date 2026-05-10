using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Produto>> GetAtivosAsync()
        => await _db.Produtos.OrderBy(e => e.Nome).ToListAsync();

    public async Task<IEnumerable<Produto>> GetComEstoqueBaixoAsync()
        => await _db.Produtos
            .Where(e => e.MinEstoque.HasValue && e.Estoque <= e.MinEstoque.Value)
            .ToListAsync();

    public async Task<PagedResult<Produto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var query = from p in _db.Produtos
                    join f in _db.Fabricantes.IgnoreQueryFilters() on p.Fabricante_Id equals f.Id into fj
                    from f in fj.DefaultIfEmpty()
                    where string.IsNullOrEmpty(search)
                          || EF.Functions.Like(p.Nome, $"%{search}%")
                          || (f != null && EF.Functions.Like(f.Nome, $"%{search}%"))
                    select p;

        int totalRecords    = await _db.Produtos.CountAsync();
        int filteredRecords = await query.CountAsync();

        query = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("estoque",    "desc") => query.OrderByDescending(e => e.Estoque),
            ("estoque",    _)      => query.OrderBy(e => e.Estoque),
            ("precovenda", "desc") => query.OrderByDescending(e => e.PrecoVenda),
            ("precovenda", _)      => query.OrderBy(e => e.PrecoVenda),
            (_,            "desc") => query.OrderByDescending(e => e.Nome),
            _                      => query.OrderBy(e => e.Nome)
        };

        var data = await query.Skip(start).Take(length).ToListAsync();

        return new PagedResult<Produto> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public override async Task<int> InsertAsync(Produto entity)
    {
        entity.Deletado = false;
        _db.Produtos.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Produto entity)
    {
        _db.Produtos.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        await _db.Produtos.IgnoreQueryFilters()
            .Where(e => e.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Deletado, true));
    }
}
