using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IFabricanteService
{
    Task<FabricanteDto?> GetByIdAsync(int id);
    Task<IEnumerable<FabricanteDto>> GetAllAsync();
    Task<PagedResult<FabricanteDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(FabricanteDto dto);
    Task UpdateAsync(FabricanteDto dto);
    Task DeleteAsync(int id);
}
