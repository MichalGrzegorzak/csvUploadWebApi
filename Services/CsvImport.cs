using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace csvUploadServices;

public class CsvImport
{
    public IList<T> ReadCsv<T>(Stream file)
    {
        var reader = new StreamReader(file);
        var csv = new CsvReader(reader, GetCsvConfiguration());
        
        var records = csv.GetRecords<T>().ToList();
        return records;
    }
    
    private static CsvConfiguration GetCsvConfiguration(bool hasHeader = true)
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeader,
        };
    }
}