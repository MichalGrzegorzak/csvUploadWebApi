using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Z.Dapper.Plus;

namespace csvUploadServices.CallsCsvImport;

public interface ICallsCsvImport
{
    Task<int> CallsCsvImportBatch(Stream fileStreamStream);
    Task<int> UploadCallCsvImportBulk(Stream fileStream);
    Task<int> UploadCallCsvImportBulk2(Stream fileStream);
}

public class CallsCsvImport : ICallsCsvImport
{
    private readonly DapperContext _context;
    const int BatchSize = 100_000;
    
    public CallsCsvImport(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> CallsCsvImportBatch(Stream fileStream)
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
    
    public async Task<int> UploadCallCsvImportBulk(Stream fileStream)
    {
        var connection = _context.CreateConnection();
        var csv = GetCsvReader(fileStream);
        
        var totalRecords = 0;
        var dataList = new List<CallData>();
        
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<CallCsvRecord>();
            
            var data = ParseCallCsvRecordToCallData(record);

            dataList.Add(data);
            totalRecords++;
        }
        connection.BulkInsert(dataList);
        
        return totalRecords;
    }
    
    public async Task<int> UploadCallCsvImportBulk2(Stream fileStream)
    {
        var connection = _context.CreateConnection();
        var csv = GetCsvReader(fileStream);
        
        var dataList = new List<CallData>();
        
        await foreach (var record in csv.GetRecordsAsync<CallCsvRecord>())
        {
            dataList.Add(ParseCallCsvRecordToCallData(record));
        }
        connection.BulkInsert(dataList);
        
        return dataList.Count();
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