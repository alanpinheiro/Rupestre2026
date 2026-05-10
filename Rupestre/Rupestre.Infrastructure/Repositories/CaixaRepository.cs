using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class CaixaRepository : BaseRepository<Caixa>, ICaixaRepository
{
    public CaixaRepository(ApplicationDbContext db) : base(db) { }

    public async Task<Caixa?> GetCaixaAbertoAsync()
        => await _db.Caixas.FirstOrDefaultAsync(c => c.OnOff);

    public override async Task<int> InsertAsync(Caixa entity)
    {
        _db.Caixas.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Caixa entity)
    {
        _db.Caixas.Update(entity);
        await _db.SaveChangesAsync();
    }
}
