using System.Reflection;
using csvUploadDomain.Context;
using csvUploadDomain.Migrations;
using csvUploadServices;
using FluentMigrator.Runner;

namespace csvUploadApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		
		public void ConfigureServices(IServiceCollection services)
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

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
		
		private ServiceProvider CreateServices()
		{
			return new ServiceCollection()
				.AddFluentMigratorCore()
				.ConfigureRunner(rb => rb
					.AddSqlServer()
					.WithGlobalConnectionString(Configuration.GetConnectionString("SqlConnection"))
					.ScanIn(typeof(AddCallData).Assembly).For.Migrations())
				// Enable logging to console in the FluentMigrator way
				.AddLogging(lb => lb.AddFluentMigratorConsole())
				// Build the service provider
				.BuildServiceProvider(false);
		}

		void UpdateDatabase(IServiceProvider serviceProvider)
		{
			var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
			runner.MigrateUp();
		}
	}
}
