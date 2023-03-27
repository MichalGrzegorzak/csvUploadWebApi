using csvUploadDomain.Extensions;
using csvUploadServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest.IntegrationTests;

[Collection("Sequence")]
public class CallRepositoryTest : IClassFixture<DatabaseFixture>
{
    private readonly ICallsRepository _callsRepository;

    public CallRepositoryTest(DatabaseFixture injection)
    {
        _callsRepository = injection.ServiceProvider.GetService<ICallsRepository>() ?? throw new InvalidOperationException();
    }

    [Theory]
    [InlineData("01/01/0001", "31/12/9999 23:59", 13718.087666)]
    [InlineData("01/01/0001", null, 13718.087666)]
    [InlineData("01/01/0001", "23/03/2023", 0)]
    [InlineData("01/01/0001", "24/03/2023", 0.833333)]
    [InlineData("01/01/0001", "25/03/2023", 13718.087666)]
    public async Task GetAvgCallCost(string dateFrom, string dateTo, decimal expected)
    {
        var (from, to) = ParseToDates(dateFrom, dateTo);
        
        var result = await _callsRepository.GetAvgCallCost(from, to);

        result.Should().Be(expected);
    }

    private static (DateTime from, DateTime? to) ParseToDates(string fromDate, string toDate)
    {
        var from = fromDate.ParseDateTime("dd/MM/yyyy HH:mm").Value;
        var to = toDate.ParseDateTime("dd/MM/yyyy HH:mm");
        return (from, to);
    }
}