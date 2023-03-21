using csvUploadDomain.Entities;
using csvUploadServices;
using Microsoft.AspNetCore.Mvc;

namespace csvUploadApi.Controllers;

[ApiController]
[Route("api/calls")]
public class CallsController : ControllerBase
{
    private readonly ILogger<CallsController> _logger;
    private readonly ICallRepository _callRepo;

    public CallsController(ILogger<CallsController> logger, ICallRepository callRepo)
    {
        _logger = logger;
        _callRepo = callRepo;
    }

    // [HttpGet(Name = "GetCalls")]
    // public IEnumerable<CallData> Get()
    // {
    //     return Enumerable.Range(1, 5).Select(index => new CallData
    //     {
    //         Reference = Summaries[index]
    //     })
    //     .ToArray();
    // }
    
    [HttpGet]
    public async Task<IActionResult> GetCalls()
    {
        try
        {
            var calls = await _callRepo.GetCallData();
            return Ok(calls);
        }
        catch (Exception ex)
        {
            //log error
            return StatusCode(500, ex.Message);
        }
    }
}
