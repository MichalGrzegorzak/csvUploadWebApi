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
public class CallsUploadTest : IClassFixture<DatabaseFixture>
{
    private readonly ICallsCsvImport _callsCsvImport;
    private readonly ITestOutputHelper _output;
    
    private const int FileSizeMultiplier = 100;
    private const string FilePath = "./TestData/techtest_cdr.csv";
    private const string BigFilePath = "./TestData/techtest_cdr_BIG.csv";

    public CallsUploadTest(DatabaseFixture injection, ITestOutputHelper output)
    {
        _output = output;
        _callsCsvImport = injection.ServiceProvider.GetService<ICallsCsvImport>() ?? throw new InvalidOperationException();
        
        //fresh table for each test 
        injection.Db.TruncateTable("CallData");
        
        Task.Run(PrepareBigFileForTests).Wait();
    }
    
    private async Task PrepareBigFileForTests()
    {
        if (File.Exists(BigFilePath))
            return;
        
        var allText = File.ReadAllLines(FilePath);
        var headerLine = allText[0];
        var allLines = allText.Skip(1).ToList();

        await File.WriteAllTextAsync(BigFilePath, headerLine);

        foreach (var _ in Enumerable.Range(1, FileSizeMultiplier))
        {
            await File.AppendAllLinesAsync(BigFilePath, allLines);
        }
    }

    [Fact]
    public async Task T01_CallsCsvImportPerBatchTest()
    {
        var stream = File.OpenRead(BigFilePath);
        
        var result = await _callsCsvImport.CallsCsvImportPerBatch(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
        _output.WriteLine(result.ToString());
    }
    
    [Fact]
    public async Task T02_UploadCallCsvImportInOneGoTest()
    {
        var stream = File.OpenRead(BigFilePath);
        
        var result = await _callsCsvImport.UploadCallCsvImportInOneGo(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
        _output.WriteLine(result.ToString());
    }
    
    [Fact]
    public async Task T03_UploadCallCsvImportByRecordTest()
    {
        var stream = File.OpenRead(BigFilePath);
        
        var result = await _callsCsvImport.UploadCallCsvImportByRecord(stream);
        
        result.Should().Be(13035*FileSizeMultiplier-1);
        _output.WriteLine(result.ToString());
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
