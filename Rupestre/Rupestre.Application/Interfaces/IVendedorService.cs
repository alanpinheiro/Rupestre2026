using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IVendedorService
{
    Task<VendedorDto?> GetByIdAsync(int id);
    Task<IEnumerable<VendedorDto>> GetAllAsync();
    Task<PagedResult<VendedorDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(VendedorDto dto);
    Task UpdateAsync(VendedorDto dto);
    Task DeleteAsync(int id);
}
