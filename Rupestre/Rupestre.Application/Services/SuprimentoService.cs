using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class SuprimentoService : ISuprimentoService
{
    private readonly ISuprimentoRepository _repository;
    private readonly ICaixaRepository _caixaRepository;

    public SuprimentoService(ISuprimentoRepository repository, ICaixaRepository caixaRepository)
    {
        _repository = repository;
        _caixaRepository = caixaRepository;
    }

    public async Task<SuprimentoDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<PagedResult<SuprimentoDto>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, orderColumn, orderDir);
        return new PagedResult<SuprimentoDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(SuprimentoDto dto)
    {
        var caixa = await _caixaRepository.GetCaixaAbertoAsync()
            ?? throw new InvalidOperationException("Não há caixa aberto. Abra o caixa antes de lançar um suprimento.");

        return await _repository.InsertAsync(new Suprimento
        {
            ValorSuprimento = dto.ValorSuprimento,
            Caixa_Id = caixa.Id
        });
    }

    public async Task UpdateAsync(SuprimentoDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Suprimento não encontrado.");

        existing.ValorSuprimento = dto.ValorSuprimento;
        await _repository.UpdateAsync(existing);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static SuprimentoDto ToDto(Suprimento e) => new()
    {
        Id = e.Id,
        ValorSuprimento = e.ValorSuprimento,
        Caixa_Id = e.Caixa_Id
    };
}
