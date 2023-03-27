using csvUploadDomain.Entities;
using FluentMigrator;

namespace csvUploadDomain.Migrations;

[Migration(202303211711)]
public class SeedCallData : Migration
{
    public override void Down()
    {
        //to delete individual records
        // Delete.FromTable("CallData").Row(new CallData { Id = new Guid("67fbac34-1ee1-4697-b916-1748861dd275") });
        
        Delete.FromTable("CallData").Row(null).AllRows();
    }

    public override void Up()
    {
        var today = new DateTime(2023, 3, 25);
        var yesterday = today.AddDays(-1);
        var twoDaysAgo = today.AddDays(-2);
        
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0, 1, "0100200300", "0400500600", twoDaysAgo));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0, 2, "0100200300", "0400500600", twoDaysAgo));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0, 3, "0400500600", "0100200300", twoDaysAgo));
        
        Insert.IntoTable("CallData")
            .Row(CreateCallData(2, 10, "0100200300", "0400500600", yesterday));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(3, 10, "0100200300", "0400500600", yesterday));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0m, 10, "0400500600", "0100200300", yesterday));
        
        Insert.IntoTable("CallData")
            .Row(CreateCallData(123456.789m, 3600*25, "0100200300", "0400500600", today));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(1.0m, 100, "0100200300", "0400500600", today));
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0m, 10, "0400500600", "0100200300", today));
    }

    public static CallData CreateCallData(decimal cost, int seconds, string from, string to, DateTime date)
    {
        return new CallData
        {
            Id = Guid.NewGuid(),
            Cost = cost,
            Currency = "GBP",
            CallStart = date,
            CallEnd = date.AddSeconds(seconds),
            Duration = seconds,
            CallerId = from,
            Recipient = to,
            Reference = $"ref-{to}"
        };
    }
}
