using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IVendedorRepository : IRepository<Vendedor>
{
    Task<IEnumerable<Vendedor>> GetAtivosAsync();
    Task<PagedResult<Vendedor>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
