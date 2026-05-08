using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IVendaProdutoRepository : IRepository<VendaProduto>
{
    Task<IEnumerable<VendaProduto>> GetByVendaAsync(int vendaId);
    Task DeleteByVendaAsync(int vendaId);
}
