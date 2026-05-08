using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class VendedorService : IVendedorService
{
    private readonly IVendedorRepository _repository;

    public VendedorService(IVendedorRepository repository) => _repository = repository;

    public async Task<VendedorDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<IEnumerable<VendedorDto>> GetAllAsync()
    {
        var list = await _repository.GetAtivosAsync();
        return list.Select(ToDto);
    }

    public async Task<PagedResult<VendedorDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<VendedorDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(VendedorDto dto)
        => await _repository.InsertAsync(new Vendedor { Nome = dto.Nome });

    public async Task UpdateAsync(VendedorDto dto)
        => await _repository.UpdateAsync(new Vendedor { Id = dto.Id, Nome = dto.Nome });

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static VendedorDto ToDto(Vendedor e) => new() { Id = e.Id, Nome = e.Nome };
}
