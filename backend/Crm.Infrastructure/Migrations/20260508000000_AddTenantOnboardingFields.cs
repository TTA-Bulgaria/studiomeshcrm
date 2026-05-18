using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Migrations
{
    public partial class AddTenantOnboardingFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandColor",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanySize",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessAddress",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetMonthlyRevenue",
                table: "Tenants",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnboardingCompleted",
                table: "Tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BrandColor", table: "Tenants");
            migrationBuilder.DropColumn(name: "Industry", table: "Tenants");
            migrationBuilder.DropColumn(name: "CompanySize", table: "Tenants");
            migrationBuilder.DropColumn(name: "Website", table: "Tenants");
            migrationBuilder.DropColumn(name: "BusinessAddress", table: "Tenants");
            migrationBuilder.DropColumn(name: "TargetMonthlyRevenue", table: "Tenants");
            migrationBuilder.DropColumn(name: "OnboardingCompleted", table: "Tenants");
        }
    }
}
