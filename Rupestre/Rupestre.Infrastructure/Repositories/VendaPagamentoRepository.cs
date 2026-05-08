using Dapper;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaPagamentoRepository : BaseRepository<VendaPagamento>, IVendaPagamentoRepository
{
    protected override string TableName => "VendaPagamento";

    public VendaPagamentoRepository(ConnectionManager cm) : base(cm) { }

    public async Task<IEnumerable<VendaPagamento>> GetByVendaAsync(int vendaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<VendaPagamento>(
            "SELECT * FROM VendaPagamento WHERE Venda_Id = @VendaId",
            new { VendaId = vendaId });
    }

    public async Task DeleteByVendaAsync(int vendaId)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "DELETE FROM VendaPagamento WHERE Venda_Id = @VendaId",
            new { VendaId = vendaId });
    }

    public override async Task<int> InsertAsync(VendaPagamento e)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO VendaPagamento (Venda_Id, FormaPagamento_Id, TipoPagamento_Id, Parcelas, ValorPagamento, ValorLiquidoPagamento, Caixa_Id)
              VALUES (@Venda_Id, @FormaPagamento_Id, @TipoPagamento_Id, @Parcelas, @ValorPagamento, @ValorLiquidoPagamento, @Caixa_Id);
              SELECT SCOPE_IDENTITY();",
            e);
    }

    public override async Task UpdateAsync(VendaPagamento e)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE VendaPagamento SET Parcelas=@Parcelas, ValorPagamento=@ValorPagamento, ValorLiquidoPagamento=@ValorLiquidoPagamento WHERE Id=@Id",
            e);
    }
}
