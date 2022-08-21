using FluentMigrator;

namespace VecoBackend.Migrations;

[Migration(202201011201)]
public class _202201011201_CreateTables : Migration
{
    public override void Up()
    {
        Create.Table("Test").InSchema("public")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity().NotNullable()
            .WithColumn("IntField").AsInt32().Nullable()
            .WithColumn("StringField").AsString().Nullable()
            .WithColumn("DateField").AsDate().Nullable()
            .WithColumn("DateTimeField").AsDateTime().Nullable()
            .WithColumn("TimeField").AsTime().Nullable()
            .WithColumn("BitField").AsBoolean().Nullable()
            .WithColumn("Deleted").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Test");
    }
}