using System.Globalization;

namespace csvUploadServices;

public sealed class CallCsvRecordMap : CsvHelper.Configuration.ClassMap<CallCsvRecord>
{
    public CallCsvRecordMap()
    {
        var dateFormat = "dd/MM/yyyy";
        var timeFormat = "HH:mm:ss";
        var enGb = CultureInfo.GetCultureInfo("en-GB");

        Map(m => m.CallerId).Index(0);
        Map(m => m.Recipient).Index(1);
        Map(m => m.CallDate).Index(2)
            .TypeConverterOption.Format(dateFormat).TypeConverterOption.CultureInfo(enGb);
        Map(m => m.EndTime).Index(3)
            .TypeConverterOption.Format(timeFormat).TypeConverterOption.CultureInfo(enGb);
        Map(m => m.Duration).Index(4);
        Map(m => m.Cost).Index(5);
        Map(m => m.Reference).Index(6);
        Map(m => m.Currency).Index(7);
    }
}