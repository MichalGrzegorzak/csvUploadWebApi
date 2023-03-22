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

    [Fact]
    public async Task GetAvgCallCost()
    {
        var result = await _callRepository.GetAvgCallCost(DateTime.MinValue, DateTime.MaxValue);
        result.Should().Be(41152.304000M);
    }
}