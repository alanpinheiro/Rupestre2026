using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly IFabricanteRepository _fabricanteRepository;

    public ProdutoService(IProdutoRepository repository, IFabricanteRepository fabricanteRepository)
    {
        _repository = repository;
        _fabricanteRepository = fabricanteRepository;
    }

    public async Task<ProdutoDto?> GetByIdAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id);
        if (e is null) return null;
        var fab = await _fabricanteRepository.GetByIdAsync(e.Fabricante_Id);
        return ToDto(e, fab?.Nome ?? string.Empty);
    }

    public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
    {
        var list = await _repository.GetAtivosAsync();
        var fabricantes = (await _fabricanteRepository.GetAtivosAsync()).ToDictionary(f => f.Id, f => f.Nome);
        return list.Select(p => ToDto(p, fabricantes.TryGetValue(p.Fabricante_Id, out var n) ? n : string.Empty));
    }

    public async Task<PagedResult<ProdutoDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        var fabricantes = (await _fabricanteRepository.GetAtivosAsync()).ToDictionary(f => f.Id, f => f.Nome);
        return new PagedResult<ProdutoDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(p => ToDto(p, fabricantes.TryGetValue(p.Fabricante_Id, out var n) ? n : string.Empty))
        };
    }

    public async Task<int> CreateAsync(ProdutoDto dto)
        => await _repository.InsertAsync(ToEntity(dto));

    public async Task UpdateAsync(ProdutoDto dto)
        => await _repository.UpdateAsync(ToEntity(dto));

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static ProdutoDto ToDto(Produto e, string nomeFabricante) => new()
    {
        Id = e.Id, Nome = e.Nome, MinEstoque = e.MinEstoque, MaxEstoque = e.MaxEstoque,
        Estoque = e.Estoque, PrecoCusto = e.PrecoCusto, PrecoVenda = e.PrecoVenda,
        Fabricante_Id = e.Fabricante_Id, NomeFabricante = nomeFabricante
    };

    private static Produto ToEntity(ProdutoDto dto) => new()
    {
        Id = dto.Id, Nome = dto.Nome, MinEstoque = dto.MinEstoque, MaxEstoque = dto.MaxEstoque,
        Estoque = dto.Estoque, PrecoCusto = dto.PrecoCusto, PrecoVenda = dto.PrecoVenda,
        Fabricante_Id = dto.Fabricante_Id
    };
}
