using csvUploadApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace csvUploadTest;

public class InjectionFixture : IDisposable
{
    private readonly TestServer server;
    private readonly HttpClient client;

    public InjectionFixture()
    {
        var path = AppContext.BaseDirectory;
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var webHostBuilder = new WebHostBuilder();
        //webHostBuilder.ConfigureServices(s => s.AddDbContext<DatabaseContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))));
        webHostBuilder.UseConfiguration(configuration);
        webHostBuilder.UseStartup<TestsStartup>();

        server = new TestServer(webHostBuilder);
        client = server.CreateClient();
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