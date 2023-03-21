using csvUploadDomain.Context;
using FluentMigrator.Runner;

namespace csvUploadApi.Extensions;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
        databaseService.DropDatabase("CsvUpload");
        databaseService.CreateDatabase("CsvUpload");

        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        migrationService.ListMigrations();
        
        try
        {
            migrationService.MigrateUp();
        }
        catch (Exception)
        {
            //we can revert to last stable
            migrationService.MigrateDown(202303211434);
        }
        

        return host;
    }
}