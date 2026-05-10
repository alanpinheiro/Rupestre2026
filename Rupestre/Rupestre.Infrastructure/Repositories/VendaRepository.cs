using Microsoft.EntityFrameworkCore;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
    public VendaRepository(ApplicationDbContext db) : base(db) { }

    public async Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId)
        => await _db.Vendas.Where(v => v.Cliente_Id == clienteId).ToListAsync();

    public async Task<IEnumerable<Venda>> GetByCaixaAsync(int caixaId)
        => await _db.Vendas.Where(v => v.Caixa_Id == caixaId).ToListAsync();

    public async Task<PagedResult<VendaDetalhe>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        int? searchId = int.TryParse(search, out var parsed) ? parsed : null;

        var baseQuery = from v in _db.Vendas
                        join c in _db.Clientes.IgnoreQueryFilters() on v.Cliente_Id equals c.Id into cj
                        from c in cj.DefaultIfEmpty()
                        join ve in _db.Vendedores.IgnoreQueryFilters() on v.Vendedor_Id equals ve.Id into vej
                        from ve in vej.DefaultIfEmpty()
                        where string.IsNullOrEmpty(search)
                              || (c != null && EF.Functions.Like(c.Nome, $"%{search}%"))
                              || (searchId.HasValue && v.Id == searchId.Value)
                        select new VendaDetalhe
                        {
                            Id                  = v.Id,
                            NumeroPedidoExterno = v.NumeroPedidoExterno,
                            DataVenda           = v.DataVenda,
                            StatusVenda         = v.StatusVenda,
                            DataEntrega         = v.DataEntrega,
                            StatusEntrega       = v.StatusEntrega,
                            Observacao          = v.Observacao,
                            Frete               = v.Frete,
                            ResidenciaPagamento = v.ResidenciaPagamento,
                            Caixa_Id            = v.Caixa_Id,
                            Cliente_Id          = v.Cliente_Id,
                            Vendedor_Id         = v.Vendedor_Id,
                            NomeCliente         = c != null ? c.Nome : "",
                            NomeVendedor        = ve != null ? ve.Nome : "",
                            TotalVenda          = _db.VendaProdutos
                                .Where(vp => vp.Venda_Id == v.Id)
                                .Sum(vp => (decimal?)vp.SubTotal) ?? 0
                        };

        int totalRecords    = await _db.Vendas.CountAsync();
        int filteredRecords = await baseQuery.CountAsync();

        IQueryable<VendaDetalhe> ordered = (orderColumn.ToLower(), orderDir.ToLower()) switch
        {
            ("id",        "asc")  => baseQuery.OrderBy(v => v.Id),
            ("id",        _)      => baseQuery.OrderByDescending(v => v.Id),
            ("datavenda", "asc")  => baseQuery.OrderBy(v => v.DataVenda),
            (_,           "desc") => baseQuery.OrderByDescending(v => v.DataVenda),
            _                     => baseQuery.OrderByDescending(v => v.DataVenda)
        };

        var data = await ordered.Skip(start).Take(length).ToListAsync();

        return new PagedResult<VendaDetalhe> { Data = data, TotalRecords = totalRecords, FilteredRecords = filteredRecords };
    }

    public async Task<(VendaDetalhe? Venda, IEnumerable<VendaProduto> Produtos, IEnumerable<VendaPagamento> Pagamentos)> GetByIdWithDetailsAsync(int id)
    {
        var venda = await (from v in _db.Vendas
                           join c in _db.Clientes.IgnoreQueryFilters() on v.Cliente_Id equals c.Id into cj
                           from c in cj.DefaultIfEmpty()
                           join ve in _db.Vendedores.IgnoreQueryFilters() on v.Vendedor_Id equals ve.Id into vej
                           from ve in vej.DefaultIfEmpty()
                           where v.Id == id
                           select new VendaDetalhe
                           {
                               Id                  = v.Id,
                               NumeroPedidoExterno = v.NumeroPedidoExterno,
                               DataVenda           = v.DataVenda,
                               StatusVenda         = v.StatusVenda,
                               DataEntrega         = v.DataEntrega,
                               StatusEntrega       = v.StatusEntrega,
                               Observacao          = v.Observacao,
                               Frete               = v.Frete,
                               ResidenciaPagamento = v.ResidenciaPagamento,
                               Caixa_Id            = v.Caixa_Id,
                               Cliente_Id          = v.Cliente_Id,
                               Vendedor_Id         = v.Vendedor_Id,
                               NomeCliente         = c != null ? c.Nome : "",
                               NomeVendedor        = ve != null ? ve.Nome : "",
                               TotalVenda          = _db.VendaProdutos
                                   .Where(vp => vp.Venda_Id == v.Id)
                                   .Sum(vp => (decimal?)vp.SubTotal) ?? 0
                           }).FirstOrDefaultAsync();

        var produtos   = await _db.VendaProdutos.Where(vp => vp.Venda_Id == id).ToListAsync();
        var pagamentos = await _db.VendaPagamentos.Where(vp => vp.Venda_Id == id).ToListAsync();

        return (venda, produtos, pagamentos);
    }

    public async Task<int> CreateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.Vendas.Add(venda);
            await _db.SaveChangesAsync();

            foreach (var p in produtos)
            {
                p.Venda_Id = venda.Id;
                _db.VendaProdutos.Add(p);
            }
            foreach (var pg in pagamentos)
            {
                pg.Venda_Id = venda.Id;
                _db.VendaPagamentos.Add(pg);
            }
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return venda.Id;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.VendaProdutos.Where(vp => vp.Venda_Id == venda.Id).ExecuteDeleteAsync();
            await _db.VendaPagamentos.Where(vp => vp.Venda_Id == venda.Id).ExecuteDeleteAsync();

            _db.Vendas.Update(venda);
            await _db.SaveChangesAsync();

            foreach (var p in produtos)
            {
                p.Venda_Id = venda.Id;
                _db.VendaProdutos.Add(p);
            }
            foreach (var pg in pagamentos)
            {
                pg.Venda_Id = venda.Id;
                _db.VendaPagamentos.Add(pg);
            }
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public override async Task<int> InsertAsync(Venda entity)
    {
        _db.Vendas.Add(entity);
        await _db.SaveChangesAsync();
        return entity.Id;
    }

    public override async Task UpdateAsync(Venda entity)
    {
        _db.Vendas.Update(entity);
        await _db.SaveChangesAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.VendaProdutos.Where(vp => vp.Venda_Id == id).ExecuteDeleteAsync();
            await _db.VendaPagamentos.Where(vp => vp.Venda_Id == id).ExecuteDeleteAsync();

            var venda = await _db.Vendas.FindAsync(id);
            if (venda is not null) _db.Vendas.Remove(venda);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
