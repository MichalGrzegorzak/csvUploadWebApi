using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Dapper;

namespace csvUploadServices;

public interface ICallRepository
{
    public string LetsTest();
    public Task<IEnumerable<CallData>> GetCallData();
}

public class CallRepository : ICallRepository
{
    private readonly DapperContext _context;

    public CallRepository(DapperContext context)
    {
        _context = context;
    }
    
    public string LetsTest()
    {
        return "success";
    }
    
    public async Task<IEnumerable<CallData>> GetCallData()
    {
        var query = "SELECT * FROM CallData";

        using var connection = _context.CreateConnection();
        
        var companies = await connection.QueryAsync<CallData>(query);
        return companies.ToList();
    }
}