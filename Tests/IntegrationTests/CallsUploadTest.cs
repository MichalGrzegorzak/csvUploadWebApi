using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using csvUploadDomain.Context;
using csvUploadServices.CallsCsvImport;
using csvUploadTest.Benchmarks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace csvUploadTest.IntegrationTests;

[Collection("Sequence")]
public class CallsUploadTest : IClassFixture<DiFixture>
{
    private readonly ICallsCsvImport _callsCsvImport;
    private readonly ITestOutputHelper _output;
    private readonly Database _database;
    private const int FileSizeMultiplier = 100;

    public CallsUploadTest(DiFixture di, ITestOutputHelper output)
    {
        _output = output;
        _callsCsvImport = di.ServiceProvider.GetService<ICallsCsvImport>() ?? throw new InvalidOperationException();
        _database = di.ServiceProvider.GetService<Database>();
        
        //fresh table for each test 
        _database.TruncateTable("CallData");
        
        Task.Run(PrepareBigFileForTests).Wait();
    }
    
    private async Task PrepareBigFileForTests()
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
    public async Task T01_CallsCsvImportPerBatchTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
        var result = await _callsCsvImport.CallsCsvImportPerBatch(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
    }
    
    [Fact]
    public async Task T02_UploadCallCsvImportInOneGoTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
        var result = await _callsCsvImport.UploadCallCsvImportInOneGo(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
    }
    
    [Fact]
    public async Task T03_UploadCallCsvImportByRecordTest()
    {
        var path = "./TestData/techtest_cdr_BIG.csv";
        var stream = File.OpenRead(path);
        
          var result = await _callsCsvImport.UploadCallCsvImportByRecord(stream);
        
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
