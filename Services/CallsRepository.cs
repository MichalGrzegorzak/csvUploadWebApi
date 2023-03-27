using System.Data.SqlTypes;
using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Dapper;

namespace csvUploadServices;

public interface ICallsRepository
{
    Task<IEnumerable<CallData>> GetCallsData(DateTime from, DateTime? to = null);
    Task<IEnumerable<CallData>> GetXLongestCalls(int xCalls, DateTime from, DateTime? to = null);
    Task<IEnumerable<DateCount>> GetNumberOfCallsPerDay(DateTime from, DateTime? to = null);
    
    Task<decimal?> GetAvgNumberOfCalls(DateTime from, DateTime? to = null);
    Task<decimal?> GetAvgCallCost(DateTime from, DateTime? to = null);
}

public class CallsRepository : ICallsRepository
{
    private readonly DapperContext _context;

    public CallsRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CallData>> GetCallsData(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT * FROM CallData " +
                    "WHERE CallStart BETWEEN @from AND @to";

        using var connection = _context.CreateConnection();
        
        var calls = await connection.QueryAsync<CallData>(query, new { from, to});
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
    
    public async Task<IEnumerable<DateCount>> GetNumberOfCallsPerDay(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "SELECT CAST(CallStart as DATE) as 'Date', count(*) as 'Count' from CallData " +
                    "WHERE CallStart BETWEEN @from AND @to " +
                    "group by CAST(CallStart as DATE);";

        using var connection = _context.CreateConnection();
        
        var dateCounts = await connection.QueryAsync<DateCount>(query, new { from, to});
        return dateCounts.ToList();
    }
    
    public async Task<decimal?> GetAvgNumberOfCalls(DateTime from, DateTime? to = null)
    {
        (from, to) = AdjustDatesForDb(from, to);
        
        var query = "select AVG(CntPerDay) from (" +
            " select count(d.Id) as CntPerDay from CallData d group by CAST(d.CallStart as DATE) " +
            ") t";

        using var connection = _context.CreateConnection();
        
        var average = await connection.QuerySingleAsync<decimal?>(query, new { from, to});
        return average;
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
    
    private static (DateTime from, DateTime? to) AdjustDatesForDb(DateTime from, DateTime? to = null)
    {
        to ??= SqlDateTime.MaxValue.Value;
        
        if (from == DateTime.MinValue)
            from = SqlDateTime.MinValue.Value;
        
        if (to == DateTime.MinValue)
            to = SqlDateTime.MinValue.Value;

        return (from, to);
    }
}

public class DateCount
{
    public DateCount()
    {
    }
    public DateCount(DateTime Date, int Count)
    {
        this.Date = Date;
        this.Count = Count;
    }

    public DateTime Date { get; init; }
    public int Count { get; init; }

    public void Deconstruct(out DateTime Date, out int Count)
    {
        Date = this.Date;
        Count = this.Count;
    }
}
