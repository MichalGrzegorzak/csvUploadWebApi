using csvUploadDomain.Context;
using csvUploadDomain.Extensions;
using csvUploadServices.CallsCsvImport;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest.UnitTests;

public class CallCsvImportTest : IClassFixture<InjectionFixture>
{
    private readonly CallsCsvImport _callsCsvImport;

    public CallCsvImportTest(InjectionFixture injection)
    {
        var dapperContext = injection.ServiceProvider.GetService<DapperContext>() ?? throw new InvalidOperationException();
        _callsCsvImport = new CallsCsvImport(dapperContext );
    }
    
    /// <summary>
    /// Testing CallStart calculations  
    /// </summary>
    [Theory]
    [InlineData(60, "00:01", "01/01/2000 00:00:00")]
    [InlineData(70, "00:01", "31/12/1999 23:59:50")]
    [InlineData(180, "00:00", "31/12/1999 23:57:00")]
    public void ParseCallCsvRecordToCallDataTest(int duration, string endTime, string expectedStartDate)
    {
        var expectedDate = expectedStartDate.ParseDateTime();
        
        var record = new CallCsvRecord
        {
            Duration = duration,
            CallDate = new DateOnly(2000, 1, 1),
            EndTime = TimeOnly.ParseExact(endTime, "HH:mm"),
        };
        
        var callData = _callsCsvImport.ParseCallCsvRecordToCallData(record);

        callData.CallStart.Should().Be(expectedDate);
    }


}