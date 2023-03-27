using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using csvUploadApi;
using csvUploadServices.CallsCsvImport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(SingleRunConfig))]
public class CallsCsvImportBenchmarks
{
    private ICallsCsvImport _callsCsvImport;
    private const string _path = "./TestData/techtest_cdr.csv";

    [GlobalSetup]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var webHostBuilder = new WebHostBuilder();
        webHostBuilder.UseConfiguration(configuration);
        webHostBuilder.UseStartup<Startup>();
        var host = webHostBuilder.Build();
        var services = host.Services;
        
        _callsCsvImport = services.GetService<ICallsCsvImport>();
    }

    [Benchmark]
    public async Task Upload1()
    {
        await _callsCsvImport.CallsCsvImportPerBatch(File.OpenRead(_path));
    }
    [Benchmark]
    public async Task Upload2()
    {
        await _callsCsvImport.UploadCallCsvImportInOneGo(File.OpenRead(_path));
    }
    
    [Benchmark]
    public async Task Upload3()
    {
        await _callsCsvImport.UploadCallCsvImportByRecord(File.OpenRead(_path));
    }
}

public class SingleRunConfig : ManualConfig
{
    public SingleRunConfig()
    {
        Add(DefaultConfig.Instance);

        Add(Job.Default
                .WithLaunchCount(1)     // benchmark process will be launched only once
                //.WithIterationTime(TimeInterval.FromMilliseconds(100)) // 100ms per iteration
                .WithWarmupCount(1) 
                .WithInvocationCount(16) //16 is min
        );
    }
}