using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface ISuprimentoRepository : IRepository<Suprimento>
{
    Task<IEnumerable<Suprimento>> GetByCaixaAsync(int caixaId);
    Task<PagedResult<Suprimento>> GetPagedAsync(int start, int length, string orderColumn, string orderDir);
}
