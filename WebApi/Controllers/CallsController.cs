using csvUploadDomain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace csvUploadApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CallsController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<CallsController> _logger;

    public CallsController(ILogger<CallsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetCalls")]
    public IEnumerable<CallData> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new CallData
        {
            Reference = Summaries[index]
        })
        .ToArray();
    }
}
