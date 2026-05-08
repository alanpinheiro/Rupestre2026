using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository) => _repository = repository;

    public async Task<ClienteDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<IEnumerable<ClienteDto>> GetAllAsync()
    {
        var list = await _repository.GetAtivosAsync();
        return list.Select(ToDto);
    }

    public async Task<PagedResult<ClienteDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<ClienteDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(ClienteDto dto)
    {
        var entity = new Cliente
        {
            Nome = dto.Nome, Cpf = dto.Cpf, Rg = dto.Rg,
            Endereco = dto.Endereco, Telefone1 = dto.Telefone1, Telefone2 = dto.Telefone2
        };
        return await _repository.InsertAsync(entity);
    }

    public async Task UpdateAsync(ClienteDto dto)
    {
        var entity = new Cliente
        {
            Id = dto.Id, Nome = dto.Nome, Cpf = dto.Cpf, Rg = dto.Rg,
            Endereco = dto.Endereco, Telefone1 = dto.Telefone1, Telefone2 = dto.Telefone2
        };
        await _repository.UpdateAsync(entity);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static ClienteDto ToDto(Cliente e) => new()
    {
        Id = e.Id, Nome = e.Nome, Cpf = e.Cpf, Rg = e.Rg,
        Endereco = e.Endereco, Telefone1 = e.Telefone1, Telefone2 = e.Telefone2
    };
}
