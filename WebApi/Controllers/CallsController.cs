using csvUploadDomain.Entities;
using csvUploadServices;
using Microsoft.AspNetCore.Mvc;

namespace csvUploadApi.Controllers;

[ApiController]
[Route("api/calls")]
public class CallsController : ControllerBase
{
    private readonly ILogger<CallsController> _logger;
    private readonly ICallsRepository _callsRepo;
    private readonly ICallsCsvImport _callsCsvImport;

    public CallsController(ILogger<CallsController> logger, ICallsRepository callsRepo, ICallsCsvImport callsCsvImport)
    {
        _logger = logger;
        _callsRepo = callsRepo;
        _callsCsvImport = callsCsvImport;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalls([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var calls = await _callsRepo.GetCallsData(dto.From, dto.To);
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
            var calls = await _callsRepo.GetXLongestCalls(topXcalls, dto.From, dto.To);
            return Ok(calls);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetXLongestCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetNumberOfCallsPerDay")]
    public async Task<IActionResult> GetNumberOfCallsPerDay([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var dateCounts = await _callsRepo.GetNumberOfCallsPerDay(dto.From, dto.To);
            return Ok(dateCounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNumberOfCallsPerDay error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetAvgNumberOfCalls")]
    public async Task<IActionResult> GetAvgNumberOfCalls([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var average = await _callsRepo.GetAvgNumberOfCalls(dto.From, dto.To);
            return Ok(average);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAvgNumberOfCalls error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet]
    [Route("GetAvgCallCost")]
    public async Task<IActionResult> GetAvgCallCost([FromQuery]CallsRequestDto dto)
    {
        try
        {
            var avgCallCost = await _callsRepo.GetAvgCallCost(dto.From, dto.To);
            return Ok(avgCallCost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAvgCallCost error");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("UploadCsvPerBatch")]
    public async Task<IActionResult> UploadCsvPerBatch([FromForm] IFormFileCollection files)
    {
        try
        {
            var fileStream = files[0].OpenReadStream();
            var result = await _callsCsvImport.UploadCallCsvImport(fileStream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadCsv");
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost]
    [Route("UploadCsvBulk")]
    public async Task<IActionResult> UploadCsvBulk([FromForm] IFormFileCollection files)
    {
        try
        {
            var fileStream = files[0].OpenReadStream();
            var result = _callsCsvImport.UploadCallCsvImportBulk(fileStream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadCsv2");
            return StatusCode(500, ex.Message);
        }
    }
}

public record CallsRequestDto(DateTime From, DateTime? To);
