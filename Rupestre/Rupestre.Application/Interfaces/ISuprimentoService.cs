using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface ISuprimentoService
{
    Task<SuprimentoDto?> GetByIdAsync(int id);
    Task<PagedResult<SuprimentoDto>> GetPagedAsync(int start, int length, string orderColumn, string orderDir);
    Task<int> CreateAsync(SuprimentoDto dto);
    Task UpdateAsync(SuprimentoDto dto);
    Task DeleteAsync(int id);
}
