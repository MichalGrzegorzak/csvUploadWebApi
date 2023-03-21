using System.Data;
using Dapper;

namespace csvUploadDomain.Context;

public class Database
{
    private readonly DapperContext _context;

    public Database(DapperContext context)
    {
        _context = context;
    }

    private bool DbExists(IDbConnection connection, string dbName)
    {
        var query = "SELECT * FROM sys.databases WHERE name = @name";
        
        var parameters = new DynamicParameters();
        parameters.Add("name", dbName);

        var records = connection.Query(query, parameters);
        return records.Any();
    }

    public void CreateDatabase(string dbName)
    {
        using var connection = _context.CreateMasterConnection();

        if (!DbExists(connection, dbName))
            connection.Execute($"CREATE DATABASE {dbName}");
    }

    public void DropDatabase(string dbName)
    {
        using var connection = _context.CreateMasterConnection();

        if (DbExists(connection, dbName))
            connection.Execute($"ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                               $"DROP DATABASE {dbName}");
    }
}
