using System.Data;
using Microsoft.Data.SqlClient;

namespace Rupestre.Infrastructure.Data;

public class ConnectionManager
{
    private readonly string _connectionString;

    public ConnectionManager(string connectionString) => _connectionString = connectionString;

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
