using csvUploadApi.Extensions;
using csvUploadDomain.Context;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest.IntegrationTests;

public class DatabaseFixture : DiFixture
{
    public DatabaseFixture()
    {
        Dapper = ServiceProvider.GetRequiredService<DapperContext>();
        Db = new Database(Dapper);

        var name = "CsvUploadTest";
        //Db.DropDatabase(name);
        //Db.CreateDatabase(name);
        
        MigrationManager.Migrate(name, ServiceProvider);
    }

    public void Dispose()
    {
        // ... clean up test data from the database ...
    }

    public Database Db { get; private set; }
    public DapperContext Dapper { get; private set; }
}