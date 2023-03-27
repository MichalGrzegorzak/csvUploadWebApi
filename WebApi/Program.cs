using csvUploadApi.Extensions;

namespace csvUploadApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args)
				.Build()
				.MigrateDatabase()
				.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
					webBuilder.ConfigureKestrel(serverOptions =>
					{
						serverOptions.Limits.MaxRequestBodySize = 200_000_000;
					});
					
				});
	}
}
