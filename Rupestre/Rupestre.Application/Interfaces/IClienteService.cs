using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IClienteService
{
    Task<ClienteDto?> GetByIdAsync(int id);
    Task<IEnumerable<ClienteDto>> GetAllAsync();
    Task<PagedResult<ClienteDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(ClienteDto dto);
    Task UpdateAsync(ClienteDto dto);
    Task DeleteAsync(int id);
}
