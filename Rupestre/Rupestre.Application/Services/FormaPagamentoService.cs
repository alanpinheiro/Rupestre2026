using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class FormaPagamentoService : IFormaPagamentoService
{
    private readonly IFormaPagamentoRepository _repository;

    public FormaPagamentoService(IFormaPagamentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<FormaPagamentoDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<IEnumerable<FormaPagamentoDto>> GetAtivosAsync()
    {
        var list = await _repository.GetAtivosAsync();
        return list.Select(ToDto);
    }

    public async Task<PagedResult<FormaPagamentoDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<FormaPagamentoDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(FormaPagamentoDto dto)
    {
        return await _repository.InsertAsync(ToEntity(dto));
    }

    public async Task UpdateAsync(FormaPagamentoDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Forma de pagamento não encontrada.");

        existing.Nome = dto.Nome;
        existing.TaxaAdministrativa = dto.TaxaAdministrativa;
        existing.Fator1 = dto.Fator1;
        existing.Fator2 = dto.Fator2;
        existing.Fator3 = dto.Fator3;
        existing.Fator4 = dto.Fator4;
        existing.Fator5 = dto.Fator5;
        existing.Fator6 = dto.Fator6;
        existing.Fator7 = dto.Fator7;
        existing.Fator8 = dto.Fator8;
        existing.Fator9 = dto.Fator9;
        existing.Fator10 = dto.Fator10;
        existing.Fator11 = dto.Fator11;
        existing.Fator12 = dto.Fator12;

        await _repository.UpdateAsync(existing);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static FormaPagamentoDto ToDto(FormaPagamento e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        TaxaAdministrativa = e.TaxaAdministrativa,
        Fator1 = e.Fator1,
        Fator2 = e.Fator2,
        Fator3 = e.Fator3,
        Fator4 = e.Fator4,
        Fator5 = e.Fator5,
        Fator6 = e.Fator6,
        Fator7 = e.Fator7,
        Fator8 = e.Fator8,
        Fator9 = e.Fator9,
        Fator10 = e.Fator10,
        Fator11 = e.Fator11,
        Fator12 = e.Fator12
    };

    private static FormaPagamento ToEntity(FormaPagamentoDto dto) => new()
    {
        Id = dto.Id,
        Nome = dto.Nome,
        TaxaAdministrativa = dto.TaxaAdministrativa,
        Fator1 = dto.Fator1,
        Fator2 = dto.Fator2,
        Fator3 = dto.Fator3,
        Fator4 = dto.Fator4,
        Fator5 = dto.Fator5,
        Fator6 = dto.Fator6,
        Fator7 = dto.Fator7,
        Fator8 = dto.Fator8,
        Fator9 = dto.Fator9,
        Fator10 = dto.Fator10,
        Fator11 = dto.Fator11,
        Fator12 = dto.Fator12
    };
}
