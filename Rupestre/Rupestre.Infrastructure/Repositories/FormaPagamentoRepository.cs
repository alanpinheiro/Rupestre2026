using Dapper;
using Rupestre.Domain.Common;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class FormaPagamentoRepository : BaseRepository<FormaPagamento>, IFormaPagamentoRepository
{
    protected override string TableName => "FormaPagamento";

    private static readonly Dictionary<string, string> OrderColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = "Nome",
        ["taxaAdministrativa"] = "TaxaAdministrativa"
    };

    public FormaPagamentoRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<IEnumerable<FormaPagamento>> GetAtivosAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<FormaPagamento>(
            "SELECT * FROM FormaPagamento WHERE Deletado = 0 ORDER BY Nome");
    }

    public async Task<PagedResult<FormaPagamento>> GetPagedAsync(int start, int length, string search, string orderColumn, string orderDir)
    {
        var col = OrderColumns.TryGetValue(orderColumn, out var c) ? c : "Nome";
        var dir = orderDir.Equals("desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";
        var searchParam = $"%{search}%";

        var sql = $@"
SELECT * FROM FormaPagamento
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam)
ORDER BY {col} {dir}
OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY;

SELECT COUNT(*) FROM FormaPagamento WHERE Deletado = 0;

SELECT COUNT(*) FROM FormaPagamento
WHERE Deletado = 0
  AND (@Search = '' OR Nome LIKE @SearchParam);";

        using var conn = Connection;
        using var multi = await conn.QueryMultipleAsync(sql,
            new { Search = search, SearchParam = searchParam, Start = start, Length = length });

        return new PagedResult<FormaPagamento>
        {
            Data = (await multi.ReadAsync<FormaPagamento>()).ToList(),
            TotalRecords = await multi.ReadSingleAsync<int>(),
            FilteredRecords = await multi.ReadSingleAsync<int>()
        };
    }

    public override async Task<int> InsertAsync(FormaPagamento e)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(@"
            INSERT INTO FormaPagamento
                (Nome, TaxaAdministrativa, Fator1, Fator2, Fator3, Fator4, Fator5, Fator6,
                 Fator7, Fator8, Fator9, Fator10, Fator11, Fator12, Deletado)
            VALUES
                (@Nome, @TaxaAdministrativa, @Fator1, @Fator2, @Fator3, @Fator4, @Fator5, @Fator6,
                 @Fator7, @Fator8, @Fator9, @Fator10, @Fator11, @Fator12, 0);
            SELECT SCOPE_IDENTITY();", e);
    }

    public override async Task UpdateAsync(FormaPagamento e)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(@"
            UPDATE FormaPagamento SET
                Nome = @Nome, TaxaAdministrativa = @TaxaAdministrativa,
                Fator1 = @Fator1, Fator2 = @Fator2, Fator3 = @Fator3, Fator4 = @Fator4,
                Fator5 = @Fator5, Fator6 = @Fator6, Fator7 = @Fator7, Fator8 = @Fator8,
                Fator9 = @Fator9, Fator10 = @Fator10, Fator11 = @Fator11, Fator12 = @Fator12
            WHERE Id = @Id", e);
    }

    public override async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync("UPDATE FormaPagamento SET Deletado = 1 WHERE Id = @Id", new { Id = id });
    }
}
