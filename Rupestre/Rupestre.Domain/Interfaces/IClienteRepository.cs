using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<IEnumerable<Cliente>> GetAtivosAsync();
    Task<PagedResult<Cliente>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
