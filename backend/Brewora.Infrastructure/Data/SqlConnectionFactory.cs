using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Brewora.Infrastructure.Data;

public class SqlConnectionFactory
{
    private readonly string _connectionString;
    public SqlConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServer is missing");
    }

    public SqlConnection Create() => new(_connectionString);
}
