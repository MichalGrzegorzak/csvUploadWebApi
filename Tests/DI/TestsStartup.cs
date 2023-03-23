using System.Reflection;
using csvUploadApi;
using csvUploadDomain.Context;
using csvUploadDomain.Migrations;
using csvUploadServices;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest;

public class TestsStartup
{
    public TestsStartup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }
    
    public IConfiguration Configuration { get; init; }

    // we could override if need to
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<DapperContext>();
        services.AddSingleton<Database>();
			
        services.AddScoped<ICallsRepository, CallsRepository>();

        services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddSqlServer2016()
                .WithGlobalConnectionString(Configuration.GetConnectionString("SqlConnection"))
                .ScanIn(typeof(AddCallData).Assembly).For.Migrations());

        //services.AddControllers();
        services.AddControllers().AddApplicationPart(Assembly.Load("csvUploadApi")).AddControllersAsServices();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
    
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
    // public override void Configure(IApplicationBuilder app)
}