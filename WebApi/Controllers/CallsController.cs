using csvUploadServices;
using csvUploadServices.BackgroundTask;
using csvUploadServices.CallsCsvImport;
using Microsoft.AspNetCore.Mvc;

namespace csvUploadApi.Controllers;

[ApiController]
[Route("api/calls")]
public class CallsController : ControllerBase
{
    private readonly ILogger<CallsController> _logger;
    private readonly ICallsRepository _callsRepo;
    private readonly ICallsCsvImport _callsCsvImport;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;

    public CallsController(ILogger<CallsController> logger, ICallsRepository callsRepo, ICallsCsvImport callsCsvImport, IBackgroundTaskQueue backgroundTaskQueue)
    {
        _logger = logger;
        _callsRepo = callsRepo;
        _callsCsvImport = callsCsvImport;
        _backgroundTaskQueue = backgroundTaskQueue;
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
    //[DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadCsvPerBatch([FromForm] IFormFileCollection files)
    {
        var filePath = await SaveFileToTempFolder(files[0]);
        
        _backgroundTaskQueue.EnqueueTask(async (serviceScopeFactory, _) =>
        {
            using var scope = serviceScopeFactory.CreateScope();
            var importer = scope.ServiceProvider.GetRequiredService<ICallsCsvImport>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CallsController>>();
            await using var stream = System.IO.File.Open(filePath, new FileStreamOptions { Options = FileOptions.DeleteOnClose });
            
            try
            {
                await importer.CallsCsvImportPerBatch(stream);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "UploadCallCsvImport failed");
            }
        });
        
        return Ok($"Import started in background, filePath:{filePath}");
    }
    
    [HttpPost]
    [Route("UploadCsvBulk")]
    public async Task<IActionResult> UploadCsvBulk([FromForm] IFormFileCollection files)
    {
        try
        {
            var fileStream = files[0].OpenReadStream();
            var result = await _callsCsvImport.UploadCallCsvImportInOneGo(fileStream);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UploadCsv2");
            return StatusCode(500, ex.Message);
        }
    }
    
    static async Task<string> SaveFileToTempFolder(IFormFile file)
    {
        var filePath = Path.GetTempFileName();

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await file.CopyToAsync(fileStream);
        return filePath;
    }
}

public record CallsRequestDto(DateTime From, DateTime? To);
