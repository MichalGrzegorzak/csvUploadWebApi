using System.Data.SqlTypes;
using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Dapper;

namespace csvUploadServices;

public interface ICallRepository
{
    Task<IEnumerable<CallData>> GetCallsData(DateTime from, DateTime? to = null);
    Task<IEnumerable<CallData>> GetXLongestCalls(int xCalls, DateTime from, DateTime? to = null);
    Task<IEnumerable<DateCount>> GetDailyAvgNumberOfCalls(DateTime from, DateTime? to = null);
    Task<decimal?> GetAvgCallCost(DateTime from, DateTime? to = null);
    Task<UploadInfo> UploadCsv();
}

public class CallRepository : ICallRepository
{
    private readonly DapperContext _context;

    public CallRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CallData>> GetCallsData(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT * FROM CallData " +
                    "WHERE CallStart BETWEEN @from AND @to";

        using var connection = _context.CreateConnection();
        
        var calls = await connection.QueryAsync<CallData>(query, new { from, to.Value});
        return calls.ToList();
    }
    
    public async Task<IEnumerable<CallData>> GetXLongestCalls(int xCalls, DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT top(@xCalls) * FROM CallData " +
                    "WHERE CallStart BETWEEN @from AND @to " +
                    "ORDER BY Duration DESC";

        using var connection = _context.CreateConnection();
        
        var calls = await connection.QueryAsync<CallData>(query, new { from, to, xCalls});
        return calls.ToList();
    }
    
    public async Task<IEnumerable<DateCount>> GetDailyAvgNumberOfCalls(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT CAST(CallStart as DATE), count(*) as count from CallData " +
                    "WHERE CallStart BETWEEN @from AND @to " +
                    "group by CAST(CallStart as DATE);";

        using var connection = _context.CreateConnection();
        
        var dateCounts = await connection.QueryAsync<DateCount>(query, new { from, to});
        return dateCounts.ToList();
    }
    
    public async Task<decimal?> GetAvgCallCost(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT AVG(Cost) FROM CallData " +
                    "WHERE CallStart BETWEEN @from AND @to";

        using var connection = _context.CreateConnection();
        
        var average = await connection.QuerySingleAsync<decimal?>(query, new { from, to});
        return average;
    }
    
    public async Task<UploadInfo> UploadCsv()
    {
        return new UploadInfo(Success: true, Records: 123, Message: null);
    }
    
    private static (DateTime from, DateTime? to) AdjustDatesForDb(DateTime from, DateTime? to = null)
    {
        to ??= SqlDateTime.MaxValue.Value;
        
        if (from == DateTime.MinValue)
            from = SqlDateTime.MinValue.Value;

        return (from, to);
    }
}

public record DateCount(DateTime Date, int Count);
public record UploadInfo(bool Success, int Records, string? Message);