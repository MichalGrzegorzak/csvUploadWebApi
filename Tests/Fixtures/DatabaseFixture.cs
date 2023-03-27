using csvUploadApi.Extensions;
using csvUploadDomain.Context;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest.IntegrationTests;

/// <summary>
/// Init's database for tests
/// </summary>
public class DatabaseFixture : DiFixture
{
    public DatabaseFixture() : base()
    {
        Dapper = ServiceProvider.GetRequiredService<DapperContext>();
        Db = new Database(Dapper);

        InitDatabase(ServiceProvider);
    }

    public Database Db { get; private set; }
    public DapperContext Dapper { get; private set; }
    
    private void InitDatabase(IServiceProvider provider)
    {
        var dbName = "CsvUploadTest";
        Db.DropDatabase(dbName);
        Db.CreateDatabase(dbName);
        MigrationManager.Migrate(dbName, provider);
    }
}