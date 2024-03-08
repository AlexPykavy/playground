using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.EF.OptimisticLocking.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TemperatureC = table.Column<int>(type: "INTEGER", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: true),
                    row_version = table.Column<int>(type: "INTEGER", rowVersion: true, nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forecasts", x => x.Id);
                });

            migrationBuilder.Sql(@"
CREATE TRIGGER UpdateForecastsVersion
AFTER UPDATE ON Forecasts
BEGIN
    UPDATE Forecasts
    SET row_version = row_version + 1
    WHERE rowid = NEW.rowid;
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER UpdateForecastsVersion;");
            migrationBuilder.DropTable(
                name: "Forecasts");
        }
    }
}
