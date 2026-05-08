using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IDespesaService
{
    Task<DespesaDto?> GetByIdAsync(int id);
    Task<PagedResult<DespesaDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(DespesaDto dto);
    Task UpdateAsync(DespesaDto dto);
    Task DeleteAsync(int id);
}
