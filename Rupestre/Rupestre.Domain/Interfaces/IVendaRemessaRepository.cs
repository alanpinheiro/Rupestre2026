using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IVendaRemessaRepository : IRepository<VendaRemessa>
{
    Task<IEnumerable<VendaRemessa>> GetByCaixaAsync(int caixaId);
    Task<PagedResult<VendaRemessa>> GetPagedAsync(int start, int length, string orderColumn, string orderDir);
}
