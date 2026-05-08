using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class CaixaService : ICaixaService
{
    private readonly ICaixaRepository _repository;

    public CaixaService(ICaixaRepository repository) => _repository = repository;

    public async Task<CaixaDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        return e is null ? null : ToDto(e);
    }

    public async Task<CaixaDto?> GetCaixaAbertoAsync()
    {
        var e = await _repository.GetCaixaAbertoAsync();
        return e is null ? null : ToDto(e);
    }

    public async Task<int> AbrirCaixaAsync(decimal valorAbertura)
    {
        var aberto = await _repository.GetCaixaAbertoAsync();
        if (aberto is not null)
            throw new InvalidOperationException("Já existe um caixa aberto.");

        var entity = new Caixa
        {
            DataAbertura = DateTime.Now,
            OnOff = true,
            ValorAbertura = valorAbertura,
            DinheiroNoCaixa = valorAbertura
        };
        return await _repository.InsertAsync(entity);
    }

    public async Task FecharCaixaAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Caixa não encontrado.");

        entity.OnOff = false;
        await _repository.UpdateAsync(entity);
    }

    private static CaixaDto ToDto(Caixa e) => new()
    {
        Id = e.Id,
        DataAbertura = e.DataAbertura,
        OnOff = e.OnOff,
        ValorAbertura = e.ValorAbertura,
        DinheiroNoCaixa = e.DinheiroNoCaixa
    };
}
