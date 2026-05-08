using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class FabricanteService : IFabricanteService
{
    private readonly IFabricanteRepository _repository;

    public FabricanteService(IFabricanteRepository repository) => _repository = repository;

    public async Task<FabricanteDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<IEnumerable<FabricanteDto>> GetAllAsync()
    {
        var list = await _repository.GetAtivosAsync();
        return list.Select(ToDto);
    }

    public async Task<PagedResult<FabricanteDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<FabricanteDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(FabricanteDto dto)
        => await _repository.InsertAsync(new Fabricante { Nome = dto.Nome });

    public async Task UpdateAsync(FabricanteDto dto)
        => await _repository.UpdateAsync(new Fabricante { Id = dto.Id, Nome = dto.Nome });

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static FabricanteDto ToDto(Fabricante e) => new() { Id = e.Id, Nome = e.Nome };
}
