using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendaRemessaRepository : BaseRepository<VendaRemessa>, IVendaRemessaRepository
{
    protected override string TableName => "VendaRemessa";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = "Id",
        ["valorRemessa"] = "ValorRemessa",
        ["caixa_Id"] = "Caixa_Id"
    };

    public VendaRemessaRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<VendaRemessa>> GetByCaixaAsync(int caixaId)
    {
        using var conn = Connection;
        return await conn.QueryAsync<VendaRemessa>(
            "SELECT * FROM VendaRemessa WHERE Caixa_Id = @CaixaId AND Deletado = 0 ORDER BY Id DESC",
            new { CaixaId = caixaId });
    }

    public async Task<PagedResult<VendaRemessa>> GetPagedAsync(int start, int length, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "Id";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var sql = $@"
SELECT * FROM VendaRemessa
WHERE Deletado = 0
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM VendaRemessa WHERE Deletado = 0;

SELECT COUNT(*) FROM VendaRemessa WHERE Deletado = 0;";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Start = start, Length = length });

        return new PagedResult<VendaRemessa>
        {
            Data = (await multi.ReadAsync<VendaRemessa>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(VendaRemessa entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            @"INSERT INTO VendaRemessa (ValorRemessa, Caixa_Id, Deletado)
              VALUES (@ValorRemessa, @Caixa_Id, 0);
              SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(VendaRemessa entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE VendaRemessa SET ValorRemessa = @ValorRemessa WHERE Id = @Id",
            entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE VendaRemessa SET Deletado = 1 WHERE Id = @Id", new { Id = id });
    }
}
