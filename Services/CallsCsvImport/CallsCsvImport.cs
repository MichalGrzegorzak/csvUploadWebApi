using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using csvUploadDomain.Context;
using csvUploadDomain.Entities;
using Z.Dapper.Plus;

namespace csvUploadServices;

public interface ICallsCsvImport
{
    Task<int> UploadCallCsvImport(Stream file);
    Task<int> UploadCallCsvImportBulk(Stream file);
    Task<int> UploadCallCsvImportBulk2(Stream file);
}

public class CallsCsvImport : ICallsCsvImport
{
    private readonly DapperContext _context;
    
    public CallsCsvImport(DapperContext context)
    {
        _context = context;
    }

    public async Task<int> UploadCallCsvImport(Stream file)
    {
        var batchSize = 10000;
        
        var connection = _context.CreateConnection();
        connection.UseBulkOptions(options =>
        {
            options.BatchSize = batchSize;
        });
        
        var csv = GetCsvReader(file);
        
        var totalRecords = 0;
        var batchCounter = 0;
        var dataList = new List<CallData>();
        
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<CallCsvRecord>();
            
            var data = ParseRecordToCallData(record);
            
            dataList.Add(data);
            totalRecords++;
            batchCounter++;

            if (batchCounter == batchSize)
            {
                connection.BulkInsert(dataList);
                batchCounter = 0;
                dataList = new List<CallData>();
            }
        }
        
        return totalRecords;
    }
    
    public async Task<int> UploadCallCsvImportBulk(Stream file)
    {
        var connection = _context.CreateConnection();
        var csv = GetCsvReader(file);
        
        var totalRecords = 0;
        var dataList = new List<CallData>();
        
        while (await csv.ReadAsync())
        {
            var record = csv.GetRecord<CallCsvRecord>();
            
            var data = ParseRecordToCallData(record);

            dataList.Add(data);
            totalRecords++;
        }
        connection.BulkInsert(dataList);
        
        return totalRecords;
    }
    
    public async Task<int> UploadCallCsvImportBulk2(Stream file)
    {
        var connection = _context.CreateConnection();
        var csv = GetCsvReader(file);
        
        var dataList = new List<CallData>();
        
        await foreach (var record in csv.GetRecordsAsync<CallCsvRecord>())
        {
            dataList.Add(ParseRecordToCallData(record));
        }
        connection.BulkInsert(dataList);
        
        return dataList.Count();
    }

    private CallData ParseRecordToCallData(CallCsvRecord record)
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

    private CsvReader GetCsvReader(Stream file)
    {
        var reader = new StreamReader(file);
        
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