using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class VendaRemessaService : IVendaRemessaService
{
    private readonly IVendaRemessaRepository _repository;
    private readonly ICaixaRepository _caixaRepository;

    public VendaRemessaService(IVendaRemessaRepository repository, ICaixaRepository caixaRepository)
    {
        _repository = repository;
        _caixaRepository = caixaRepository;
    }

    public async Task<VendaRemessaDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<PagedResult<VendaRemessaDto>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, orderColumn, orderDir);
        return new PagedResult<VendaRemessaDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<int> CreateAsync(VendaRemessaDto dto)
    {
        var caixa = await _caixaRepository.GetCaixaAbertoAsync()
            ?? throw new InvalidOperationException("Não há caixa aberto. Abra o caixa antes de lançar uma remessa.");

        return await _repository.InsertAsync(new VendaRemessa
        {
            ValorRemessa = dto.ValorRemessa,
            Caixa_Id = caixa.Id
        });
    }

    public async Task UpdateAsync(VendaRemessaDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Remessa não encontrada.");

        existing.ValorRemessa = dto.ValorRemessa;
        await _repository.UpdateAsync(existing);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static VendaRemessaDto ToDto(VendaRemessa e) => new()
    {
        Id = e.Id,
        ValorRemessa = e.ValorRemessa,
        Caixa_Id = e.Caixa_Id
    };
}
