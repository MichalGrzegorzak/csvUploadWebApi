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

    [HttpGet]
    public async Task<IActionResult> GetCalls(DateTime from, DateTime? to = null)
    {
        to ??= DateTime.Today.AddDays(1);
        
        try
        {
            var calls = await _callRepo.GetCallsData(from, to.Value);
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    // [HttpGet]
    // public async Task<IActionResult> GetXLongestCalls(int topXcalls, DateTime from, DateTime? to = null)
    // {
    //     to ??= DateTime.Today.AddDays(1);
    //     
    //     try
    //     {
    //         var calls = await _callRepo.GetXLongestCalls(topXcalls, from, to.Value);
    //         return Ok(calls);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "GetXLongestCalls error");
    //         return StatusCode(500, ex.Message);
    //     }
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> GetDailyAvgNumberOfCalls(DateTime from, DateTime? to = null)
    // {
    //     to ??= DateTime.Today.AddDays(1);
    //     
    //     try
    //     {
    //         var dateCounts = await _callRepo.GetDailyAvgNumberOfCalls(from, to.Value);
    //         return Ok(dateCounts);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "GetDailyAvgNumberOfCalls error");
    //         return StatusCode(500, ex.Message);
    //     }
    // }
    //
    // [HttpGet]
    // public async Task<IActionResult> GetAvgCallCost(DateTime from, DateTime? to = null)
    // {
    //     to ??= DateTime.Today.AddDays(1);
    //     
    //     try
    //     {
    //         var avgCallCost = await _callRepo.GetAvgCallCost(from, to.Value);
    //         return Ok(avgCallCost);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "GetAvgCallCost error");
    //         return StatusCode(500, ex.Message);
    //     }
    // }
    //
    [HttpPost]
    public async Task<IActionResult> UploadCsv()
    {
        try
        {
            var result = await Task.FromResult(111);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadCsv");
            return StatusCode(500, ex.Message);
        }
    }
}
