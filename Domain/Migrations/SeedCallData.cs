using csvUploadDomain.Entities;
using FluentMigrator;

namespace csvUploadDomain.Migrations;

[Migration(202303211711)]
public class SeedCallData : Migration
{
    public override void Down()
    {
        // Delete.FromTable("CallData")
        //     .Row(new CallData 
        //     {
        //         Id = new Guid("67fbac34-1ee1-4697-b916-1748861dd275"),
        //     });
        Delete.FromTable("CallData").Row(null).AllRows();
    }

    public override void Up()
    {
        Insert.IntoTable("CallData")
            .Row(CreateCallData(123456.789m, 100, "0100200300", "0400500600"));
        
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0.123m, 900, "0100200300", "0400500600"));
        
        Insert.IntoTable("CallData")
            .Row(CreateCallData(0m, 9000, "0400500600", "0100200300"));
    }

    public static CallData CreateCallData(decimal cost, int seconds, string from, string to)
    {
        var date = DateTime.Now;

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
