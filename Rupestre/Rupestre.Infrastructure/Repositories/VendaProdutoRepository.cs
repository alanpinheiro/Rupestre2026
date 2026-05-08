using Dapper;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaProdutoRepository : BaseRepository<VendaProduto>, IVendaProdutoRepository
{
    protected override string TableName => "VendaProduto";

    public VendaProdutoRepository(ConnectionManager cm) : base(cm) { }

    public async Task<IEnumerable<VendaProduto>> GetByVendaAsync(int vendaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<VendaProduto>(
            "SELECT * FROM VendaProduto WHERE Venda_Id = @VendaId",
            new { VendaId = vendaId });
    }

    public async Task DeleteByVendaAsync(int vendaId)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "DELETE FROM VendaProduto WHERE Venda_Id = @VendaId",
            new { VendaId = vendaId });
    }

    public override async Task<int> InsertAsync(VendaProduto e)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO VendaProduto (Venda_Id, Produto_Id, PrecoVenda, SubTotal, Quantidade)
              VALUES (@Venda_Id, @Produto_Id, @PrecoVenda, @SubTotal, @Quantidade);
              SELECT SCOPE_IDENTITY();",
            e);
    }

    public override async Task UpdateAsync(VendaProduto e)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE VendaProduto SET PrecoVenda=@PrecoVenda, SubTotal=@SubTotal, Quantidade=@Quantidade WHERE Id=@Id",
            e);
    }
}
