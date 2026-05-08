using System.Data;
using Dapper;
using Rupestre.Domain.Interfaces;
using Rupestre.Infrastructure.Data;

namespace Rupestre.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly ConnectionManager _connectionManager;
    protected abstract string TableName { get; }

    protected BaseRepository(ConnectionManager connectionManager) => _connectionManager = connectionManager;

    protected IDbConnection Connection => _connectionManager.CreateConnection();

    public async Task<T?> GetByIdAsync(int id)
    {
        using var conn = Connection;
        return await conn.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {TableName} WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        using var conn = Connection;
        return await conn.QueryAsync<T>($"SELECT * FROM {TableName}");
    }

    public virtual Task<int> InsertAsync(T entity) => throw new NotImplementedException();
    public virtual Task UpdateAsync(T entity) => throw new NotImplementedException();

    public virtual async Task DeleteAsync(int id)
    {
        using var conn = Connection;
        await conn.ExecuteAsync($"DELETE FROM {TableName} WHERE Id = @Id", new { Id = id });
    }
}
