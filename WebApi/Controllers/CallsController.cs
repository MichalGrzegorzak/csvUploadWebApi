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
    public async Task<IActionResult> GetCalls([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var calls = await _callRepo.GetCallsData(dto.From, dto.To);
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetXLongestCalls")]
    public async Task<IActionResult> GetXLongestCalls(int topXcalls, [FromQuery]CallsRequestDto dto)
    {
        try
        {
            var calls = await _callRepo.GetXLongestCalls(topXcalls, dto.From, dto.To);
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetXLongestCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetDailyAvgNumberOfCalls")]
    public async Task<IActionResult> GetDailyAvgNumberOfCalls([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var dateCounts = await _callRepo.GetDailyAvgNumberOfCalls(dto.From, dto.To);
            return Ok(dateCounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetDailyAvgNumberOfCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetAvgCallCost")]
    public async Task<IActionResult> GetAvgCallCost([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var avgCallCost = await _callRepo.GetAvgCallCost(dto.From, dto.To);
            return Ok(avgCallCost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAvgCallCost error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("UploadCsv")]
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

public class CallsRequestDto
{
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
