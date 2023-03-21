using csvUploadDomain.Entities;
using FluentMigrator;

namespace csvUploadDomain.Migrations;

[Migration(202303211711)]
public class SeedCallData : Migration
{
    public override void Down()
    {
        Delete.FromTable("CallData")
            .Row(new CallData 
            {
                Id = new Guid("67fbac34-1ee1-4697-b916-1748861dd275"),
            });
    }

    public override void Up()
    {
        Insert.IntoTable("CallData")
            .Row(new CallData
            {
                Id = new Guid("67fbac34-1ee1-4697-b916-1748861dd275"),
                Cost = 123456.789m,
                Currency = "GBP",
                CallStart = DateTime.Now,
                CallEnd = DateTime.Now.AddMinutes(66),
                Duration = 66*60,
                CallerId = "0100200300",
                Recipient = "123456789",
                Reference = "67fbac34"
            });
    }
}
