using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IVendaRemessaService
{
    Task<VendaRemessaDto?> GetByIdAsync(int id);
    Task<PagedResult<VendaRemessaDto>> GetPagedAsync(int start, int length, string orderColumn, string orderDir);
    Task<int> CreateAsync(VendaRemessaDto dto);
    Task UpdateAsync(VendaRemessaDto dto);
    Task DeleteAsync(int id);
}
