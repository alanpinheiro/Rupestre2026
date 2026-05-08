using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class FabricanteRepository : BaseRepository<Fabricante>, IFabricanteRepository
{
    protected override string TableName => "Fabricante";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = "Nome"
    };

    public FabricanteRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<Fabricante>> GetAtivosAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<Fabricante>("SELECT * FROM Fabricante WHERE Deletado = 0 ORDER BY Nome");
    }

    public async Task<PagedResult<Fabricante>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "Nome";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT * FROM Fabricante
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM Fabricante WHERE Deletado = 0;

SELECT COUNT(*) FROM Fabricante
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql, new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<Fabricante>
        {
            Data = (await multi.ReadAsync<Fabricante>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(Fabricante entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            "INSERT INTO Fabricante (Nome,Deletado) VALUES (@Nome,0); SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Fabricante entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Fabricante SET Nome=@Nome WHERE Id=@Id", entity);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE Fabricante SET Deletado=1 WHERE Id=@Id", new { Id = id });
    }
}
