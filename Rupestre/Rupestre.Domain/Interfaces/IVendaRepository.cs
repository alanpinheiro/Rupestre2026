using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;

namespace Rupestre.Domain.Interfaces;

public interface IVendaRepository : IRepository<Venda>
{
    Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<Venda>> GetByCaixaAsync(int caixaId);
    Task<PagedResult<VendaDetalhe>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<(VendaDetalhe? Venda, IEnumerable<VendaProduto> Produtos, IEnumerable<VendaPagamento> Pagamentos)> GetByIdWithDetailsAsync(int id);
    Task<int> CreateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos);
    Task UpdateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos);
}
