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
			services.AddSingleton<DapperContext>();
			services.AddSingleton<Database>();
			
			services.AddScoped<ICallRepository, CallRepository>();

			services.AddLogging(c => c.AddFluentMigratorConsole())
				.AddFluentMigratorCore()
				.ConfigureRunner(c => c.AddSqlServer2016()
					.WithGlobalConnectionString(Configuration.GetConnectionString("SqlConnection"))
					.ScanIn(typeof(AddCallData).Assembly).For.Migrations());

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
				app.UseSwaggerUI();
			}

			app.UseRouting();
			
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
