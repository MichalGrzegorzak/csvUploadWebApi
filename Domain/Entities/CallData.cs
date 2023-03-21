namespace csvUploadDomain.Entities;

public class CallData
{
    public Guid Id { get; set; }
    
    public string CallerId { get; set; }
    public string Recipient { get; set; }
    public DateTime CallStart { get; set; }
    public DateTime CallEnd { get; set; }
    public int Duration { get; set; } //seconds
    public decimal Cost { get; set; } //to 3 decimal places
    public string Reference { get; set; }
    public string Currency { get; set; } //iso alpha-3
}