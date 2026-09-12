using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Brewora.Infrastructure.Data;

public static class DatabaseBootstrapper
{
    public static void EnsureCreated(IConfiguration config, ILogger logger)
    {
        var appCs = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("ConnectionStrings:SqlServer is missing");

        var master = new SqlConnectionStringBuilder(appCs) { InitialCatalog = "master" };
        using (var conn = new SqlConnection(master.ConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "IF DB_ID(N'BreworaDB') IS NULL CREATE DATABASE [BreworaDB];";
            cmd.ExecuteNonQuery();
        }

        for (var i = 0; i < 10; i++)
        {
            try
            {
                using var test = new SqlConnection(appCs);
                test.Open();
                break;
            }
            catch (SqlException) when (i < 9)
            {
                Thread.Sleep(300);
            }
        }

        using (var conn = new SqlConnection(appCs))
        {
            conn.Open();
            var hasMenu = ScalarInt(conn, "SELECT COUNT(*) FROM sys.tables WHERE name = N'MenuItems'") > 0;
            if (!hasMenu)
            {
                RunScript(conn, ReadSql("Tables.sql"), skipDatabaseCreate: true);
                logger.LogInformation("BreworaDB tables created.");
            }

            RunScript(conn, ReadSql("StoredProcedures.sql"), skipDatabaseCreate: true);

            var hasCategories = hasMenu && ScalarInt(conn, "SELECT COUNT(*) FROM dbo.Categories") > 0;
            if (!hasCategories)
            {
                RunScript(conn, ReadSql("SeedData.sql"), skipDatabaseCreate: true);
                logger.LogInformation("BreworaDB catalog seeded.");
            }

            SeedUsers(conn, logger);
        }

        logger.LogInformation("BreworaDB is ready on LocalDB. Refresh SQL Server Object Explorer.");
    }

    private static void SeedUsers(SqlConnection conn, ILogger logger)
    {
        var exists = ScalarInt(conn, "SELECT COUNT(*) FROM dbo.Users WHERE Email = N'customer@email.com'") > 0;
        if (exists) return;

        using var admin = conn.CreateCommand();
        admin.CommandText = "dbo.sp_User_Create";
        admin.CommandType = System.Data.CommandType.StoredProcedure;
        admin.Parameters.AddWithValue("@RoleId", 1);
        admin.Parameters.AddWithValue("@FullName", "Brewora Host");
        admin.Parameters.AddWithValue("@Email", "nina.v@example.com");
        admin.Parameters.AddWithValue("@Phone", "+918040001200");
        admin.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword("Admin@123"));
        admin.Parameters.AddWithValue("@AvatarUrl", "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80");
        admin.ExecuteScalar();

        using var customer = conn.CreateCommand();
        customer.CommandText = "dbo.sp_User_Create";
        customer.CommandType = System.Data.CommandType.StoredProcedure;
        customer.Parameters.AddWithValue("@RoleId", 2);
        customer.Parameters.AddWithValue("@FullName", "Customer Name");
        customer.Parameters.AddWithValue("@Email", "customer@email.com");
        customer.Parameters.AddWithValue("@Phone", "+91 90000 00000");
        customer.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword("Customer@123"));
        customer.Parameters.AddWithValue("@AvatarUrl", "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80");
        customer.ExecuteScalar();
        logger.LogInformation("Demo users created (nina.v@example.com / customer@email.com).");
    }

    private static int ScalarInt(SqlConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        var value = cmd.ExecuteScalar();
        return value is int i ? i : Convert.ToInt32(value);
    }

    private static string ReadSql(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Sql", fileName);
        if (!File.Exists(path))
        {
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "database", fileName));
        }
        if (!File.Exists(path))
            throw new FileNotFoundException("SQL script not found: " + fileName, path);
        return File.ReadAllText(path);
    }

    private static void RunScript(SqlConnection conn, string script, bool skipDatabaseCreate)
    {
        foreach (var batch in SplitGo(script))
        {
            var sql = batch.Trim();
            if (sql.Length == 0) continue;
            if (skipDatabaseCreate &&
                (sql.Contains("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)
                 || sql.StartsWith("USE ", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 60;
            cmd.ExecuteNonQuery();
        }
    }

    private static IEnumerable<string> SplitGo(string script)
    {
        var batch = new System.Text.StringBuilder();
        using var reader = new StringReader(script);
        while (reader.ReadLine() is { } line)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                yield return batch.ToString();
                batch.Clear();
            }
            else
            {
                batch.AppendLine(line);
            }
        }
        if (batch.Length > 0) yield return batch.ToString();
    }
}
