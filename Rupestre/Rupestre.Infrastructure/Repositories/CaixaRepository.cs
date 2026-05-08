using Dapper;
using Rupestre.Domain.Entities;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public class CaixaRepository : BaseRepository<Caixa>, ICaixaRepository
{
    protected override string TableName => "Caixa";

    public CaixaRepository(ConnectionManager connectionManager) : base(connectionManager) { }

    public async Task<Caixa?> GetCaixaAbertoAsync()
    {
        using var conn = Connection;
        return await conn.QueryFirstOrDefaultAsync<Caixa>("SELECT * FROM Caixa WHERE OnOff = 1");
    }

    public override async Task<int> InsertAsync(Caixa entity)
    {
        using var conn = Connection;
        return await conn.ExecuteScalarAsync<int>(
            "INSERT INTO Caixa (DataAbertura, OnOff, ValorAbertura, DinheiroNoCaixa) VALUES (@DataAbertura, @OnOff, @ValorAbertura, @DinheiroNoCaixa); SELECT SCOPE_IDENTITY();",
            entity);
    }

    public override async Task UpdateAsync(Caixa entity)
    {
        using var conn = Connection;
        await conn.ExecuteAsync(
            "UPDATE Caixa SET DataAbertura=@DataAbertura, OnOff=@OnOff, ValorAbertura=@ValorAbertura, DinheiroNoCaixa=@DinheiroNoCaixa WHERE Id=@Id",
            entity);
    }
}
