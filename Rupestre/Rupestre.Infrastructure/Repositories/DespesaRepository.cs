using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class DespesaRepository : BaseRepository<Despesa>, IDespesaRepository
{
    protected override string TableName => "Despesa";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = "Nome",
        ["dataDespesa"] = "DataDespesa",
        ["valorDespesa"] = "ValorDespesa",
        ["statusDespesa"] = "StatusDespesa"
    };

    public DespesaRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Despesa>> GetByCaixaAsync(int caixaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<Despesa>(
            "SELECT * FROM Despesa WHERE Caixa_Id = @CaixaId AND Deletado = 0 ORDER BY DataDespesa DESC",
            new { CaixaId = caixaId });
    }

    public async Task<PagedResult<Despesa>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "DataDespesa";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT * FROM Despesa
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Despesa WHERE Deletado = 0;

SELECT COUNT(*) FROM Despesa
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql,
            new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<Despesa>
        {
            Data = (await multi.ReadAsync<Despesa>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(Despesa entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO Despesa (Nome, DataDespesa, ValorDespesa, StatusDespesa, Deletado, Caixa_Id)
              VALUES (@Nome, @DataDespesa, @ValorDespesa, @StatusDespesa, 0, @Caixa_Id);
              SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Despesa entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE Despesa SET Nome=@Nome, DataDespesa=@DataDespesa, ValorDespesa=@ValorDespesa, StatusDespesa=@StatusDespesa WHERE Id=@Id",
            entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Despesa SET Deletado=1 WHERE Id=@Id", new { Id = id });
    }
}
