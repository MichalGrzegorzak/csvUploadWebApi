using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace csvUploadTest;

public class InjectionFixture : IDisposable
{
    private readonly TestServer server;
    private readonly HttpClient client;

    public InjectionFixture()
    {
        server = new TestServer(new WebHostBuilder().UseStartup<TestsStartup>());
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