using csvUploadServices;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace csvUploadTest;

public class MyTests : IClassFixture<InjectionFixture>
{
    private readonly InjectionFixture _injection;
    private readonly ICallRepository _callRepository;

    public MyTests(InjectionFixture injection)
    {
        _injection = injection;
        _callRepository = injection.ServiceProvider.GetService<ICallRepository>(); 
    }

    [Fact]
    public void SomeTest()
    {
        var result = _callRepository.LetsTest();
        result.Should().Be("success2");
    }
}