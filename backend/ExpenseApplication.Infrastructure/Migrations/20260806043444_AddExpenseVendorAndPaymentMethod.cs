using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseVendorAndPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VendorName",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "VendorName",
                table: "Expenses");
        }
    }
}
