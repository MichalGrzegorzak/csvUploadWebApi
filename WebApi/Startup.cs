using System.Reflection;
using csvUploadDomain.Context;
using csvUploadDomain.Migrations;
using csvUploadServices;
using FluentMigrator.Runner;

namespace csvUploadApi
{
	public class Startup
	{
		// public Startup()
		// {
		// }
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; init; }
		
		public virtual void ConfigureServices(IServiceCollection services)
		{
			//dapper
			services.AddSingleton<DapperContext>();
			services.AddSingleton<Database>();
			
			//fluent migrator
			services.AddLogging(c => c.AddFluentMigratorConsole())
				.AddFluentMigratorCore()
				.ConfigureRunner(c => c.AddSqlServer2016()
					.WithGlobalConnectionString(Configuration.GetConnectionString("SqlConnection"))
					.ScanIn(typeof(AddCallData).Assembly).For.Migrations());
			
			//other
			services.AddScoped<ICallsRepository, CallsRepository>();
			services.AddScoped<ICallsCsvImport, CallsCsvImport>();

			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
		}

		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("v1/swagger.json", "V1 Docs");
					c.DisplayRequestDuration();
				});
			}

			app.UseRouting();
			
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
