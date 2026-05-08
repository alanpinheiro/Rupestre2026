using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IVendaService
{
    Task<VendaSaveDto?> GetByIdAsync(int id);
    Task<PagedResult<VendaDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<IEnumerable<VendaDto>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<VendaDto>> GetByCaixaAsync(int caixaId);
    Task<int> CreateAsync(VendaSaveDto dto);
    Task UpdateAsync(VendaSaveDto dto);
    Task DeleteAsync(int id);
}
