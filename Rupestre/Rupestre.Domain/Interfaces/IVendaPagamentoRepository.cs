using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IVendaPagamentoRepository : IRepository<VendaPagamento>
{
    Task<IEnumerable<VendaPagamento>> GetByVendaAsync(int vendaId);
    Task DeleteByVendaAsync(int vendaId);
}
