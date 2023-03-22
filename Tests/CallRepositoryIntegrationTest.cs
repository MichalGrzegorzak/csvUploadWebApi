using System.Globalization;
using csvUploadServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest;

public class CallRepositoryIntegrationTest : IClassFixture<InjectionFixture>
{
    private readonly InjectionFixture _injection;
    private readonly ICallRepository _callRepository;

    public CallRepositoryIntegrationTest(InjectionFixture injection)
    {
        _injection = injection;
        _callRepository = injection.ServiceProvider.GetService<ICallRepository>(); 
    }

    [Theory]
    [InlineData("01/01/0001", "31/12/9999 23:59", 41152.304000)]
    [InlineData("01/01/0001", null, 41152.304000)]
    [InlineData("01/01/0001", null, 41152.304000)]
    public async Task GetAvgCallCost(string dateFrom, string dateTo, decimal expected)
    {
        var (from, to) = ParseToDates(dateFrom, dateTo);
        
        var result = await _callRepository.GetAvgCallCost(from, to);
        
        result.Should().Be(expected);
    }
    
    private static (DateTime from, DateTime? to) ParseToDates(string fromDate, string toDate)
    {
        var from = ParseDateTime(fromDate).Value;
        var to = ParseDateTime(toDate);
        return (from, to);
    }

    private static DateTime? ParseDateTime(string inputDate, string dateFormat = "dd/MM/yyyy HH:mm")
    {
        if (string.IsNullOrWhiteSpace(inputDate))
            return null;

        if (dateFormat.Contains(' ') && !inputDate.Contains(' '))
            return DateTime.ParseExact(inputDate, dateFormat.Split(' ')[0], CultureInfo.InvariantCulture);
        
        return DateTime.ParseExact(inputDate, dateFormat, CultureInfo.InvariantCulture);
    }
}