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
        migrationService.MigrateUp();

        return host;
    }
}