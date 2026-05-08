using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProdutoDto>> GetAllAsync();
    Task<PagedResult<ProdutoDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(ProdutoDto dto);
    Task UpdateAsync(ProdutoDto dto);
    Task DeleteAsync(int id);
}
