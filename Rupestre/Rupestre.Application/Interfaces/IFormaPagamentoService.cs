using Rupestre.Application.DTOs;
using Rupestre.Domain.Common;

namespace Rupestre.Application.Interfaces;

public interface IFormaPagamentoService
{
    Task<FormaPagamentoDto?> GetByIdAsync(int id);
    Task<IEnumerable<FormaPagamentoDto>> GetAtivosAsync();
    Task<PagedResult<FormaPagamentoDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir);
    Task<int> CreateAsync(FormaPagamentoDto dto);
    Task UpdateAsync(FormaPagamentoDto dto);
    Task DeleteAsync(int id);
}
