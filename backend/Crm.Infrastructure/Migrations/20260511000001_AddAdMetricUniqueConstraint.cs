using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Migrations
{
    public partial class AddAdMetricUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AdMetrics_AdAccountId_Date",
                table: "AdMetrics",
                columns: new[] { "AdAccountId", "Date" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdMetrics_AdAccountId_Date",
                table: "AdMetrics");
        }
    }
}
