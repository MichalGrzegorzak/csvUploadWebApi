namespace csvUploadServices;

public class CallCsvImport
{
    public string CallerId { get; set; }
    public string Recipient { get; set; }
    public DateOnly CallDate { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Duration { get; set; } //seconds
    public decimal Cost { get; set; } //to 3 decimal places
    public string Reference { get; set; }
    public string Currency { get; set; } //iso alpha-3
}