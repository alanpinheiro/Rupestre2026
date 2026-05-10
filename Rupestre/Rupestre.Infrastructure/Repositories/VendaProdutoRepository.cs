using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaProdutoRepository : BaseRepository<VendaProduto>, IVendaProdutoRepository
{
    public VendaProdutoRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<VendaProduto>> GetByVendaAsync(int vendaId)
        => await _db.VendaProdutos.Where(e => e.Venda_Id == vendaId).ToListAsync();

    public async Task DeleteByVendaAsync(int vendaId)
        => await _db.VendaProdutos.Where(e => e.Venda_Id == vendaId).ExecuteDeleteAsync();

    public override async Task<int> InsertAsync(VendaProduto entity)
    {
        _db.VendaProdutos.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(VendaProduto entity)
    {
        _db.VendaProdutos.Update(entity);
        await _db.SaveChangesAsync();
    }
}
