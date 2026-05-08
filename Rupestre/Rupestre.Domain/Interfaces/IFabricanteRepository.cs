using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IFabricanteRepository : IRepository<Fabricante>
{
    Task<IEnumerable<Fabricante>> GetAtivosAsync();
    Task<PagedResult<Fabricante>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
}
