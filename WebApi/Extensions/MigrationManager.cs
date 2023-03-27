using csvUploadDomain.Context;
using FluentMigrator.Runner;

namespace csvUploadApi.Extensions;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var result = Migrate("CsvUpload", scope.ServiceProvider);

        return host;
    }
    
    public static bool Migrate(string dbName, IServiceProvider provider)
    {
        var databaseService = provider.GetRequiredService<Database>();
        //databaseService.DropDatabase(dbName);
        databaseService.CreateDatabase(dbName);

        var migrationService = provider.GetRequiredService<IMigrationRunner>();
        migrationService.ListMigrations();
        
        try
        {
            migrationService.MigrateUp();
            return true;
        }
        catch (Exception)
        {
            //we can revert to last stable
            migrationService.MigrateDown(202303211434);
        }
        return false;
    }
}