using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IDespesaRepository : IRepository<Despesa>
{
    Task<IEnumerable<Despesa>> GetByCaixaAsync(int caixaId);
    Task<PagedResult<Despesa>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
