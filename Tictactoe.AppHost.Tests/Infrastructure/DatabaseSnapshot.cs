using Microsoft.Data.SqlClient;
using System.Data;

namespace Tictactoe.AppHost.Tests.Infrastructure;

public class DatabaseSnapshot : IAsyncLifetime
{
    private const string DbName = "my-db";
    private const string DbSnapshotPath = "C:\\test\\snapshots\\";
    private const string DbSnapshotName = "my-db-snapshot";
    private const string DbConnectionString = $"Server=(localdb)\\mssqllocaldb;Database={DbName};Trusted_Connection=True;MultipleActiveResultSets=true";

    public async Task RestoreSnapshotAsync()
    {
        var sql = $"""
            USE master;
            ALTER DATABASE [{DbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{DbName}] FROM DATABASE_SNAPSHOT = '{DbSnapshotName}';
            ALTER DATABASE [{DbName}] SET MULTI_USER;
            """;

        await ExecuteSqlAsync(sql);
    }

    private async Task CreateSnapshotAsync()
    {
        if (!Directory.Exists(DbSnapshotPath))
            Directory.CreateDirectory(DbSnapshotPath);

        var sql = $"CREATE DATABASE [{DbSnapshotName}] ON (NAME=[{DbName}], FILENAME='{DbSnapshotPath}{DbSnapshotName}') AS SNAPSHOT OF [{DbName}]";

        await ExecuteSqlAsync(sql);
    }

    private async Task DeleteSnapshotAsync()
    {
        var sql = $"DROP DATABASE [{DbSnapshotName}]";

        await ExecuteSqlAsync(sql);
    }

    private async Task ExecuteSqlAsync(string sql, params SqlParameter[] parameters)
    {
        await using var conn = new SqlConnection(DbConnectionString);
        await conn.OpenAsync();
        var cmd = new SqlCommand(sql, conn) { CommandType = CommandType.Text };
        cmd.Parameters.AddRange(parameters);
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }

    public async ValueTask InitializeAsync()
    {
        try
        {
            await DeleteSnapshotAsync();
        }
        catch
        {
            // Ignore if snapshot does not exist
        }

        await CreateSnapshotAsync();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await DeleteSnapshotAsync();
    }
}
