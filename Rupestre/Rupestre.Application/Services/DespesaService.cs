using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Enums;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class DespesaService : IDespesaService
{
    private readonly IDespesaRepository _repository;
    private readonly ICaixaRepository _caixaRepository;

    public DespesaService(IDespesaRepository repository, ICaixaRepository caixaRepository)
    {
        _repository = repository;
        _caixaRepository = caixaRepository;
    }

    public async Task<DespesaDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<PagedResult<DespesaDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<DespesaDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(DespesaDto dto)
    {
        var caixa = await _caixaRepository.GetCaixaAbertoAsync()
            ?? throw new InvalidOperationException("Não há caixa aberto. Abra o caixa antes de lançar uma despesa.");

        var entity = new Despesa
        {
            Nome = dto.Nome,
            DataDespesa = dto.DataDespesa,
            ValorDespesa = dto.ValorDespesa,
            StatusDespesa = dto.StatusDespesa,
            Caixa_Id = caixa.Id
        };
        return await _repository.InsertAsync(entity);
    }

    public async Task UpdateAsync(DespesaDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Despesa não encontrada.");

        existing.Nome = dto.Nome;
        existing.DataDespesa = dto.DataDespesa;
        existing.ValorDespesa = dto.ValorDespesa;
        existing.StatusDespesa = dto.StatusDespesa;

        await _repository.UpdateAsync(existing);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static DespesaDto ToDto(Despesa e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        DataDespesa = e.DataDespesa,
        ValorDespesa = e.ValorDespesa,
        StatusDespesa = e.StatusDespesa,
        Caixa_Id = e.Caixa_Id
    };
}
