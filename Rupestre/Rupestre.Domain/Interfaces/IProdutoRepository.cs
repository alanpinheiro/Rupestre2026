using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> GetAtivosAsync();
    Task<IEnumerable<Produto>> GetComEstoqueBaixoAsync();
    Task<PagedResult<Produto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
