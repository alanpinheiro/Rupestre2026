using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class SuprimentoRepository : BaseRepository<Suprimento>, ISuprimentoRepository
{
    protected override string TableName => "Suprimento";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = "Id",
        ["valorSuprimento"] = "ValorSuprimento"
    };

    public SuprimentoRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Suprimento>> GetByCaixaAsync(int caixaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<Suprimento>(
            "SELECT * FROM Suprimento WHERE Caixa_Id = @CaixaId AND Deletado = 0 ORDER BY Id DESC",
            new { CaixaId = caixaId });
    }

    public async Task<PagedResult<Suprimento>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "Id";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var sql = $@"
SELECT * FROM Suprimento
WHERE Deletado = 0
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Suprimento WHERE Deletado = 0;

SELECT COUNT(*) FROM Suprimento WHERE Deletado = 0;";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Start = start, Length = length });

        return new PagedResult<Suprimento>
        {
            Data = (await multi.ReadAsync<Suprimento>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(Suprimento entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO Suprimento (ValorSuprimento, Caixa_Id, Deletado)
              VALUES (@ValorSuprimento, @Caixa_Id, 0);
              SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Suprimento entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE Suprimento SET ValorSuprimento = @ValorSuprimento WHERE Id = @Id",
            entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Suprimento SET Deletado = 1 WHERE Id = @Id", new { Id = id });
    }
}
