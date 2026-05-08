using Rupestre.Application.DTOs;
using Rupestre.Application.Interfaces;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;

namespace Rupestre.Application.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _repository;
    private readonly ICaixaRepository _caixaRepository;

    public VendaService(IVendaRepository repository, ICaixaRepository caixaRepository)
    {
        _repository = repository;
        _caixaRepository = caixaRepository;
    }

    public async Task<VendaSaveDto?> GetByIdAsync(int id)
    {
        var (venda, produtos, pagamentos) = await _repository.GetByIdWithDetailsAsync(id);
        if (venda is null) return null;

        return new VendaSaveDto
        {
            Id = venda.Id,
            NumeroPedidoExterno = venda.NumeroPedidoExterno,
            DataVenda = venda.DataVenda,
            StatusVenda = venda.StatusVenda,
            DataEntrega = venda.DataEntrega,
            StatusEntrega = venda.StatusEntrega,
            Observacao = venda.Observacao,
            Frete = venda.Frete,
            ResidenciaPagamento = venda.ResidenciaPagamento,
            Cliente_Id = venda.Cliente_Id,
            Vendedor_Id = venda.Vendedor_Id,
            Produtos = produtos.Select(p => new VendaProdutoDto
            {
                Id = p.Id,
                Venda_Id = p.Venda_Id,
                Produto_Id = p.Produto_Id,
                PrecoVenda = p.PrecoVenda,
                SubTotal = p.SubTotal,
                Quantidade = p.Quantidade
            }).ToList(),
            Pagamentos = pagamentos.Select(p => new VendaPagamentoDto
            {
                Id = p.Id,
                Venda_Id = p.Venda_Id,
                FormaPagamento_Id = p.FormaPagamento_Id,
                TipoPagamento_Id = p.TipoPagamento_Id,
                Parcelas = p.Parcelas,
                ValorPagamento = p.ValorPagamento,
                ValorLiquidoPagamento = p.ValorLiquidoPagamento
            }).ToList()
        };
    }

    public async Task<PagedResult<VendaDto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var result = await _repository.GetPagedAsync(start, length, search, orderColumn, orderDir);
        return new PagedResult<VendaDto>
        {
            TotalRecords = result.TotalRecords,
            FilteredRecords = result.FilteredRecords,
            Data = result.Data.Select(ToDto)
        };
    }

    public async Task<IEnumerable<VendaDto>> GetByClienteAsync(int clienteId)
    {
        var entities = await _repository.GetByClienteAsync(clienteId);
        return entities.Select(e => new VendaDto
        {
            Id = e.Id,
            DataVenda = e.DataVenda,
            StatusVenda = e.StatusVenda,
            Cliente_Id = e.Cliente_Id
        });
    }

    public async Task<IEnumerable<VendaDto>> GetByCaixaAsync(int caixaId)
    {
        var entities = await _repository.GetByCaixaAsync(caixaId);
        return entities.Select(e => new VendaDto
        {
            Id = e.Id,
            DataVenda = e.DataVenda,
            StatusVenda = e.StatusVenda,
            Caixa_Id = e.Caixa_Id
        });
    }

    public async Task<int> CreateAsync(VendaSaveDto dto)
    {
        var caixa = await _caixaRepository.GetCaixaAbertoAsync()
            ?? throw new InvalidOperationException("Não há caixa aberto. Abra o caixa antes de registrar uma venda.");

        var venda = new Venda
        {
            NumeroPedidoExterno = dto.NumeroPedidoExterno,
            DataVenda = dto.DataVenda,
            StatusVenda = dto.StatusVenda,
            DataEntrega = dto.DataEntrega,
            StatusEntrega = dto.StatusEntrega,
            Observacao = dto.Observacao,
            Frete = dto.Frete,
            ResidenciaPagamento = dto.ResidenciaPagamento,
            Caixa_Id = caixa.Id,
            Cliente_Id = dto.Cliente_Id,
            Vendedor_Id = dto.Vendedor_Id
        };

        var produtos = dto.Produtos.Select(p => new VendaProduto
        {
            Produto_Id = p.Produto_Id,
            PrecoVenda = p.PrecoVenda,
            Quantidade = p.Quantidade,
            SubTotal = p.PrecoVenda * p.Quantidade
        }).ToList();

        var pagamentos = dto.Pagamentos.Select(p => new VendaPagamento
        {
            FormaPagamento_Id = p.FormaPagamento_Id,
            TipoPagamento_Id = p.TipoPagamento_Id,
            Parcelas = p.Parcelas,
            ValorPagamento = p.ValorPagamento,
            ValorLiquidoPagamento = p.ValorLiquidoPagamento,
            Caixa_Id = caixa.Id
        }).ToList();

        return await _repository.CreateWithDetailsAsync(venda, produtos, pagamentos);
    }

    public async Task UpdateAsync(VendaSaveDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Venda não encontrada.");

        existing.NumeroPedidoExterno = dto.NumeroPedidoExterno;
        existing.DataVenda = dto.DataVenda;
        existing.StatusVenda = dto.StatusVenda;
        existing.DataEntrega = dto.DataEntrega;
        existing.StatusEntrega = dto.StatusEntrega;
        existing.Observacao = dto.Observacao;
        existing.Frete = dto.Frete;
        existing.ResidenciaPagamento = dto.ResidenciaPagamento;
        existing.Cliente_Id = dto.Cliente_Id;
        existing.Vendedor_Id = dto.Vendedor_Id;

        var produtos = dto.Produtos.Select(p => new VendaProduto
        {
            Venda_Id = dto.Id,
            Produto_Id = p.Produto_Id,
            PrecoVenda = p.PrecoVenda,
            Quantidade = p.Quantidade,
            SubTotal = p.PrecoVenda * p.Quantidade
        }).ToList();

        var pagamentos = dto.Pagamentos.Select(p => new VendaPagamento
        {
            Venda_Id = dto.Id,
            FormaPagamento_Id = p.FormaPagamento_Id,
            TipoPagamento_Id = p.TipoPagamento_Id,
            Parcelas = p.Parcelas,
            ValorPagamento = p.ValorPagamento,
            ValorLiquidoPagamento = p.ValorLiquidoPagamento,
            Caixa_Id = existing.Caixa_Id
        }).ToList();

        await _repository.UpdateWithDetailsAsync(existing, produtos, pagamentos);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);

    private static VendaDto ToDto(VendaDetalhe e) => new()
    {
        Id = e.Id,
        NumeroPedidoExterno = e.NumeroPedidoExterno,
        DataVenda = e.DataVenda,
        StatusVenda = e.StatusVenda,
        DataEntrega = e.DataEntrega,
        StatusEntrega = e.StatusEntrega,
        Observacao = e.Observacao,
        Frete = e.Frete,
        ResidenciaPagamento = e.ResidenciaPagamento,
        Caixa_Id = e.Caixa_Id,
        Cliente_Id = e.Cliente_Id,
        NomeCliente = e.NomeCliente,
        Vendedor_Id = e.Vendedor_Id,
        NomeVendedor = e.NomeVendedor,
        TotalVenda = e.TotalVenda
    };
}
