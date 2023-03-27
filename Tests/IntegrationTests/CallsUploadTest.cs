using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using csvUploadServices.CallsCsvImport;
using csvUploadTest.Benchmarks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace csvUploadTest.IntegrationTests;

[Collection("Sequence")]
public class CallsUploadTest : IClassFixture<InjectionFixture>
{
    private readonly ICallsCsvImport _callsCsvImport;
    private readonly ITestOutputHelper _output;
    private const int FileSizeMultiplier = 100;

    public CallsUploadTest(InjectionFixture injection, ITestOutputHelper output)
    {
        _output = output;
        _callsCsvImport = injection.ServiceProvider.GetService<ICallsCsvImport>() ?? throw new InvalidOperationException();

        Task.Run(PrepareBigFileForNextTests).Wait();
    }
    
    private async Task PrepareBigFileForNextTests()
    {
        var path = "./TestData/techtest_cdr.csv";
        var bigFile = "./TestData/techtest_cdr_BIG.csv";

        if (!File.Exists(bigFile))
        {
            var allText = File.ReadAllLines(path);
            var headerLine = allText[0];
            var allLines = allText.Skip(1).ToList();

            await File.WriteAllTextAsync(bigFile, headerLine);

            foreach (var _ in Enumerable.Range(1, FileSizeMultiplier))
            {
                await File.AppendAllLinesAsync(bigFile, allLines);
            }
        }
    }

    [Fact]
    public async Task T01_UploadCallCsvImportTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
        var result = await _callsCsvImport.CallsCsvImportBatch(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
    }
    
    [Fact]
    public async Task T02_UploadCallCsvImportTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
        var result = await _callsCsvImport.UploadCallCsvImportBulk(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
    }
    
    [Fact]
    public async Task T03_UploadCallCsvImportTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
        var result = await _callsCsvImport.UploadCallCsvImportBulk2(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
    }

    /// <summary>
    /// I was trying to measure memory usage of each import, but it's not a right tool
    /// </summary>
    [Fact(Skip = "Runs benchmarking, don't run - takes ages")]
    public void UploadCallCsvImportBulkTest()
    {
        var logger = new AccumulationLogger();

        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddLogger(logger)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator);

        BenchmarkRunner.Run<CallsCsvImportBenchmarks>(config);

        _output.WriteLine(logger.GetLog());
    }
}
