using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class VendedorRepository : BaseRepository<Vendedor>, IVendedorRepository
{
    protected override string TableName => "Vendedor";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = "Nome"
    };

    public VendedorRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Vendedor>> GetAtivosAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<Vendedor>("SELECT * FROM Vendedor WHERE Deletado = 0 ORDER BY Nome");
    }

    public async Task<PagedResult<Vendedor>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "Nome";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT * FROM Vendedor
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Vendedor WHERE Deletado = 0;

SELECT COUNT(*) FROM Vendedor
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<Vendedor>
        {
            Data = (await multi.ReadAsync<Vendedor>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(Vendedor entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            "INSERT INTO Vendedor (Nome,Deletado) VALUES (@Nome,0); SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Vendedor entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Vendedor SET Nome=@Nome WHERE Id=@Id", entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Vendedor SET Deletado=1 WHERE Id=@Id", new { Id = id });
    }
}
