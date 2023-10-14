using Microsoft.Data.SqlClient;

namespace HangfireScheduler
{
    public static class Bootstrapper
    {
        public static async Task CreateDatabaseAsync(string connectionString)
        {
            using var databaseConnection = new SqlConnection(connectionString);
            using var masterConnection = new SqlConnection(connectionString + ";database=master");
            using var command = masterConnection.CreateCommand();
            
            command.CommandText = $@"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = '{databaseConnection.Database}')
	CREATE DATABASE [{databaseConnection.Database}]";

            await masterConnection.OpenAsync();
            await command.ExecuteScalarAsync();
        }
    }
}
