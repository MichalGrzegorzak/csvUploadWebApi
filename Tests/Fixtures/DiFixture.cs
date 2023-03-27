using csvUploadApi;
using csvUploadApi.Extensions;
using csvUploadDomain.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest;

public class DiFixture : IDisposable
{
    private readonly TestServer server;
    private readonly HttpClient client;

    public DiFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var webHostBuilder = new WebHostBuilder();
        webHostBuilder.UseConfiguration(configuration);
        webHostBuilder.UseStartup<Startup>();

        server = new TestServer(webHostBuilder);
        client = server.CreateClient();

        var db = ServiceProvider.GetService<Database>();
        db.CreateDatabase("CsvUploadTest");
        
        MigrationManager.Migrate("CsvUploadTest", server.Services);
    }

    public IServiceProvider ServiceProvider => server.Host.Services;

    public void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            server.Dispose();
            client.Dispose();
        }
    }
}