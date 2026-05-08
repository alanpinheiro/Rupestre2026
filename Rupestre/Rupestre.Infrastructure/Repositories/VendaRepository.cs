using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
    protected override string TableName => "Venda";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = "v.Id",
        ["dataVenda"] = "v.DataVenda"
    };

    public VendaRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<Venda>(
            "SELECT * FROM Venda WHERE Cliente_Id = @ClienteId",
            new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<Venda>> GetByCaixaAsync(int caixaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<Venda>(
            "SELECT * FROM Venda WHERE Caixa_Id = @CaixaId",
            new { CaixaId = caixaId });
    }

    public async Task<PagedResult<VendaDetalhe>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "v.DataVenda";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT v.*, c.Nome AS NomeCliente, ve.Nome AS NomeVendedor,
       ISNULL((SELECT SUM(vp.SubTotal) FROM VendaProduto vp WHERE vp.Venda_Id = v.Id), 0) AS TotalVenda
FROM Venda v
LEFT JOIN Cliente c ON c.Id = v.Cliente_Id
LEFT JOIN Vendedor ve ON ve.Id = v.Vendedor_Id
WHERE (@Search = '' OR c.Nome LIKE @SearchParam OR CAST(v.Id AS VARCHAR) LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Venda;

SELECT COUNT(*) FROM Venda v
LEFT JOIN Cliente c ON c.Id = v.Cliente_Id
WHERE (@Search = '' OR c.Nome LIKE @SearchParam OR CAST(v.Id AS VARCHAR) LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql,
            new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<VendaDetalhe>
        {
            Data = (await multi.ReadAsync<VendaDetalhe>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public async Task<(VendaDetalhe? Venda, IEnumerable<VendaProduto> Produtos, IEnumerable<VendaPagamento> Pagamentos)> GetByIdWithDetailsAsync(int id)
    {
        const string sql = @"
SELECT v.*, c.Nome AS NomeCliente, ve.Nome AS NomeVendedor,
       ISNULL((SELECT SUM(vp.SubTotal) FROM VendaProduto vp WHERE vp.Venda_Id = v.Id), 0) AS TotalVenda
FROM Venda v
LEFT JOIN Cliente c ON c.Id = v.Cliente_Id
LEFT JOIN Vendedor ve ON ve.Id = v.Vendedor_Id
WHERE v.Id = @Id;

SELECT * FROM VendaProduto WHERE Venda_Id = @Id;

SELECT * FROM VendaPagamento WHERE Venda_Id = @Id;";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Id = id });

        var venda = await multi.ReadFirstOrDefaultAsync<VendaDetalhe>();
        var produtos = (await multi.ReadAsync<VendaProduto>()).ToList();
        var pagamentos = (await multi.ReadAsync<VendaPagamento>()).ToList();

        return (venda, produtos, pagamentos);
    }

    public async Task<int> CreateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos)
    {
        var conn = _connectionManager.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            var vendaId = await conn.ExecuteScalarAsync<int>(
                @"INSERT INTO Venda (NumeroPedidoExterno, DataVenda, StatusVenda, DataEntrega, StatusEntrega, Observacao, Frete, ResidenciaPagamento, Caixa_Id, Cliente_Id, Vendedor_Id)
                  VALUES (@NumeroPedidoExterno, @DataVenda, @StatusVenda, @DataEntrega, @StatusEntrega, @Observacao, @Frete, @ResidenciaPagamento, @Caixa_Id, @Cliente_Id, @Vendedor_Id);
                  SELECT SCOPE_IDENTITY();",
                venda, tx);

            foreach (var produto in produtos)
            {
                produto.Venda_Id = vendaId;
                await conn.ExecuteAsync(
                    @"INSERT INTO VendaProduto (Venda_Id, Produto_Id, PrecoVenda, SubTotal, Quantidade)
                      VALUES (@Venda_Id, @Produto_Id, @PrecoVenda, @SubTotal, @Quantidade);",
                    produto, tx);
            }

            foreach (var pagamento in pagamentos)
            {
                pagamento.Venda_Id = vendaId;
                await conn.ExecuteAsync(
                    @"INSERT INTO VendaPagamento (Venda_Id, FormaPagamento_Id, TipoPagamento_Id, Parcelas, ValorPagamento, ValorLiquidoPagamento, Caixa_Id)
                      VALUES (@Venda_Id, @FormaPagamento_Id, @TipoPagamento_Id, @Parcelas, @ValorPagamento, @ValorLiquidoPagamento, @Caixa_Id);",
                    pagamento, tx);
            }

            tx.Commit();
            return vendaId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
        finally
        {
            conn.Dispose();
        }
    }

    public async Task UpdateWithDetailsAsync(Venda venda, IEnumerable<VendaProduto> produtos, IEnumerable<VendaPagamento> pagamentos)
    {
        var conn = _connectionManager.CreateConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            await conn.ExecuteAsync(
                "DELETE FROM VendaProduto WHERE Venda_Id = @Id",
                new { venda.Id }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM VendaPagamento WHERE Venda_Id = @Id",
                new { venda.Id }, tx);

            await conn.ExecuteAsync(
                @"UPDATE Venda SET
                    NumeroPedidoExterno=@NumeroPedidoExterno,
                    DataVenda=@DataVenda,
                    StatusVenda=@StatusVenda,
                    DataEntrega=@DataEntrega,
                    StatusEntrega=@StatusEntrega,
                    Observacao=@Observacao,
                    Frete=@Frete,
                    ResidenciaPagamento=@ResidenciaPagamento,
                    Cliente_Id=@Cliente_Id,
                    Vendedor_Id=@Vendedor_Id
                  WHERE Id=@Id",
                venda, tx);

            foreach (var produto in produtos)
            {
                produto.Venda_Id = venda.Id;
                await conn.ExecuteAsync(
                    @"INSERT INTO VendaProduto (Venda_Id, Produto_Id, PrecoVenda, SubTotal, Quantidade)
                      VALUES (@Venda_Id, @Produto_Id, @PrecoVenda, @SubTotal, @Quantidade);",
                    produto, tx);
            }

            foreach (var pagamento in pagamentos)
            {
                pagamento.Venda_Id = venda.Id;
                await conn.ExecuteAsync(
                    @"INSERT INTO VendaPagamento (Venda_Id, FormaPagamento_Id, TipoPagamento_Id, Parcelas, ValorPagamento, ValorLiquidoPagamento, Caixa_Id)
                      VALUES (@Venda_Id, @FormaPagamento_Id, @TipoPagamento_Id, @Parcelas, @ValorPagamento, @ValorLiquidoPagamento, @Caixa_Id);",
                    pagamento, tx);
            }

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
        finally
        {
            conn.Dispose();
        }
    }

    public override async Task<int> InsertAsync(Venda entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO Venda (NumeroPedidoExterno, DataVenda, StatusVenda, DataEntrega, StatusEntrega, Observacao, Frete, ResidenciaPagamento, Caixa_Id, Cliente_Id, Vendedor_Id)
              VALUES (@NumeroPedidoExterno, @DataVenda, @StatusVenda, @DataEntrega, @StatusEntrega, @Observacao, @Frete, @ResidenciaPagamento, @Caixa_Id, @Cliente_Id, @Vendedor_Id);
              SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Venda entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            @"UPDATE Venda SET
                NumeroPedidoExterno=@NumeroPedidoExterno,
                DataVenda=@DataVenda,
                StatusVenda=@StatusVenda,
                DataEntrega=@DataEntrega,
                StatusEntrega=@StatusEntrega,
                Observacao=@Observacao,
                Frete=@Frete,
                ResidenciaPagamento=@ResidenciaPagamento,
                Cliente_Id=@Cliente_Id,
                Vendedor_Id=@Vendedor_Id
              WHERE Id=@Id",
            entity);
    }
}
