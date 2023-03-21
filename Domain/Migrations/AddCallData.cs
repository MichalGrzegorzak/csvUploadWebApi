using FluentMigrator;

namespace csvUploadDomain.Migrations;

[Migration(202303211434)]
public class AddCallData : Migration
{
    public override void Up()
    {
        Create.Table("CallData")
            .WithColumn("Id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("CallerId").AsString()
            .WithColumn("Recipient").AsString()
            .WithColumn("CallStart").AsDateTime()
            .WithColumn("CallEnd").AsDateTime()
            .WithColumn("Duration").AsInt32()
            .WithColumn("Cost").AsDecimal(6, 3)
            .WithColumn("Reference").AsString()
            .WithColumn("Currency").AsFixedLengthString(3);
    }

    public override void Down()
    {
        Delete.Table("CallData");
    }
}