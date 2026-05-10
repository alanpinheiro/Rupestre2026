using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaPagamentoRepository : BaseRepository<VendaPagamento>, IVendaPagamentoRepository
{
    public VendaPagamentoRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<VendaPagamento>> GetByVendaAsync(int vendaId)
        => await _db.VendaPagamentos.Where(e => e.Venda_Id == vendaId).ToListAsync();

    public async Task DeleteByVendaAsync(int vendaId)
        => await _db.VendaPagamentos.Where(e => e.Venda_Id == vendaId).ExecuteDeleteAsync();

    public override async Task<int> InsertAsync(VendaPagamento entity)
    {
        _db.VendaPagamentos.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(VendaPagamento entity)
    {
        _db.VendaPagamentos.Update(entity);
        await _db.SaveChangesAsync();
    }
}
