using csvUploadApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace csvUploadTest;

public class TestsStartup : Startup
{
    public TestsStartup(IWebHostEnvironment env) : base(null)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }

    // we could override if need to
    // public override void ConfigureServices(IServiceCollection services)
    // public override void Configure(IApplicationBuilder app)
}