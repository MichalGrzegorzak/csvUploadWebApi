using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Z.Dapper.Plus;

namespace csvUploadServices.CallsCsvImport;

public interface ICallsCsvImport
{
    Task<int> CallsCsvImportPerBatch(Stream fileStreamStream);
    Task<int> UploadCallCsvImportInOneGo(Stream fileStream);
    Task<int> UploadCallCsvImportByRecord(Stream fileStream);
}

public class CallsCsvImport : ICallsCsvImport
{
    private readonly DapperContext _context;
    private readonly ICallsRepository _repo;
    const int BatchSize = 100_000_000;
    
    public CallsCsvImport(DapperContext context, ICallsRepository repo)
    {
        _context = context;
        _repo = repo;
    }

    public async Task<int> CallsCsvImportPerBatch(Stream fileStream)
    {
        var connection = _context.CreateConnection();
        connection.UseBulkOptions(options =>
        {
            options.BatchSize = BatchSize;
        });
        
        var csv = GetCsvReader(fileStream);
        
        var totalRecords = 0;
        var batchCounter = 0;
        var dataList = new List<CallData>();
        
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<CallCsvRecord>();
            
            var data = ParseCallCsvRecordToCallData(record);
            
            dataList.Add(data);
            totalRecords++;
            batchCounter++;

            if (batchCounter == BatchSize)
            {
                connection.BulkInsert(dataList);
                batchCounter = 0;
                dataList = new List<CallData>();
            }
        }
        
        return totalRecords;
    }
    
    public async Task<int> UploadCallCsvImportInOneGo(Stream fileStream)
    {
        var connection = _context.CreateConnection();
        var csv = GetCsvReader(fileStream);
        
        var totalRecords = 0;
        var dataList = new List<CallData>();
        
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<CallCsvRecord>();
            
            dataList.Add(ParseCallCsvRecordToCallData(record));
            totalRecords++;
        }
        connection.BulkInsert(dataList);
        
        return totalRecords;
    }
    
    public async Task<int> UploadCallCsvImportByRecord(Stream fileStream)
    {
        var csv = GetCsvReader(fileStream);
        
        var totalRecords = 0;
        
        await foreach (var record in csv.GetRecordsAsync<CallCsvRecord>())
        {
            _repo.Insert(ParseCallCsvRecordToCallData(record));
            totalRecords++;
        }
        
        return totalRecords;
    }

    public CallData ParseCallCsvRecordToCallData(CallCsvRecord record)
    {
        var endDate = record.CallDate.ToDateTime(record.EndTime);
        
        var timeSpan = new TimeSpan(0, 0, 0, record.Duration);
        var timeStarted = record.EndTime.Add(-timeSpan);
        var startDate = record.CallDate.ToDateTime(timeStarted);
            
        if (record.EndTime.ToTimeSpan().TotalSeconds - record.Duration < 0)
        {
            startDate = startDate.AddDays(-1);
        }

        var data = new CallData
        {
            Id = Guid.NewGuid(),
            CallStart = startDate,
            CallEnd = endDate,
            CallerId = record.CallerId,
            Cost = record.Cost,
            Currency = record.Currency,
            Duration = record.Duration,
            Recipient = record.Recipient,
            Reference = record.Reference
        };

        return data;
    }

    private CsvReader GetCsvReader(Stream fileStream)
    {
        var reader = new StreamReader(fileStream);
        
        var csv = new CsvReader(reader, GetCsvConfiguration());
        csv.Context.RegisterClassMap<CallCsvRecordMap>();

        csv.Read();
        csv.ReadHeader();
        return csv;
    }

    private static CsvConfiguration GetCsvConfiguration(bool hasHeader = true)
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeader,
        };
    }

}