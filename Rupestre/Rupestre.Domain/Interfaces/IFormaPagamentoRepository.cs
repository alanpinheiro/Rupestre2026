using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IFormaPagamentoRepository : IRepository<FormaPagamento>
{
    Task<IEnumerable<FormaPagamento>> GetAtivosAsync();
    Task<PagedResult<FormaPagamento>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
