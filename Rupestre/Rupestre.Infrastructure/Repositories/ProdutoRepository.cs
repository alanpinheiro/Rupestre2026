using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    protected override string TableName => "Produto";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = "p.Nome", ["estoque"] = "p.Estoque", ["precoVenda"] = "p.PrecoVenda", ["nomeFabricante"] = "f.Nome"
    };

    public ProdutoRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Produto>> GetAtivosAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<Produto>("SELECT * FROM Produto WHERE Deletado = 0 ORDER BY Nome");
    }

    public async Task<IEnumerable<Produto>> GetComEstoqueBaixoAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<Produto>(
            "SELECT * FROM Produto WHERE Deletado = 0 AND MinEstoque IS NOT NULL AND Estoque <= MinEstoque");
    }

    public async Task<PagedResult<Produto>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "p.Nome";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT p.* FROM Produto p
LEFT JOIN Fabricante f ON f.Id = p.Fabricante_Id
WHERE p.Deletado = 0
  AND (@Search = '' OR p.Nome LIKE @SearchParam OR f.Nome LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Produto WHERE Deletado = 0;

SELECT COUNT(*) FROM Produto p
LEFT JOIN Fabricante f ON f.Id = p.Fabricante_Id
WHERE p.Deletado = 0
  AND (@Search = '' OR p.Nome LIKE @SearchParam OR f.Nome LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<Produto>
        {
            Data = (await multi.ReadAsync<Produto>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(Produto entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            "INSERT INTO Produto (Nome,MinEstoque,MaxEstoque,Estoque,PrecoCusto,PrecoVenda,Fabricante_Id,Deletado) VALUES (@Nome,@MinEstoque,@MaxEstoque,@Estoque,@PrecoCusto,@PrecoVenda,@Fabricante_Id,0); SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Produto entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE Produto SET Nome=@Nome,MinEstoque=@MinEstoque,MaxEstoque=@MaxEstoque,Estoque=@Estoque,PrecoCusto=@PrecoCusto,PrecoVenda=@PrecoVenda,Fabricante_Id=@Fabricante_Id WHERE Id=@Id",
            entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Produto SET Deletado=1 WHERE Id=@Id", new { Id = id });
    }
}
