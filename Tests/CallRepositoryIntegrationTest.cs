using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using csvUploadServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest;

public class CallRepositoryIntegrationTest : IClassFixture<InjectionFixture>
{
    private readonly InjectionFixture _injection;
    private readonly ICallsRepository _callsRepository;

    public CallRepositoryIntegrationTest(InjectionFixture injection)
    {
        _injection = injection;
        _callsRepository = injection.ServiceProvider.GetService<ICallsRepository>(); 
    }

    [Theory]
    [InlineData("01/01/0001", "31/12/9999 23:59", 41152.304000)]
    [InlineData("01/01/0001", null, 41152.304000)]
    [InlineData("01/01/0001", null, 41152.304000)]
    public async Task GetAvgCallCost(string dateFrom, string dateTo, decimal expected)
    {
        var (from, to) = ParseToDates(dateFrom, dateTo);
        
        var result = await _callsRepository.GetAvgCallCost(from, to);
        
        result.Should().Be(expected);

    }

    [Theory]
    [InlineData(60, "00:01", "01/01/2000 00:00:00")]
    [InlineData(70, "00:01", "31/12/1999 23:59:50")]
    [InlineData(180, "00:00", "31/12/1999 23:57:00")]
    public async Task TestStartDate(int duration, string endTime, string expectedStartDate)
    {
        var record = new CallCsvRecord()
        {
            Duration = duration,
            CallDate = new DateOnly(2000, 1, 1),
            EndTime = TimeOnly.ParseExact(endTime, "HH:mm"),
        };
        
        var timeSpan = new TimeSpan(0, 0, 0, record.Duration);
        var timeStarted = record.EndTime.Add(-timeSpan);
        var startDate = record.CallDate.ToDateTime(timeStarted);
        var endDate = record.CallDate.ToDateTime(record.EndTime);
            
        if (record.EndTime.ToTimeSpan().TotalSeconds - record.Duration < 0)
        {
            startDate = startDate.AddDays(-1);
        }

        var expectedDate = ParseDateTime(expectedStartDate).Value;
        startDate.Should().Be(expectedDate);
    }

    private static (DateTime from, DateTime? to) ParseToDates(string fromDate, string toDate)
    {
        var from = ParseDateTime(fromDate, "dd/MM/yyyy HH:mm").Value;
        var to = ParseDateTime(toDate, "dd/MM/yyyy HH:mm");
        return (from, to);
    }

    private static DateTime? ParseDateTime(string inputDate, string dateFormat = "dd/MM/yyyy HH:mm:ss")
    {
        if (string.IsNullOrWhiteSpace(inputDate))
            return null;

        if (dateFormat.Contains(' ')) //formatting contains time
        {
            if (!inputDate.Contains(' '))
                return DateTime.ParseExact(inputDate, dateFormat.Split(' ')[0], CultureInfo.InvariantCulture);

            //when no seconds
            if (dateFormat.Contains(":ss") && inputDate.Split(':').Length < 2)
                return DateTime.ParseExact(inputDate, dateFormat.Replace(":ss", ""), CultureInfo.InvariantCulture);
        }

        return DateTime.ParseExact(inputDate, dateFormat, CultureInfo.InvariantCulture);
    }
}