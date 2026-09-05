using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Expenses",
                newName: "ExpenseDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpenseDate",
                table: "Expenses",
                newName: "CreatedDate");
        }
        public partial class AddExpenseVendorAndPaymentMethod : Migration
        {
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.AddColumn<string>(
                    name: "VendorName",
                    table: "Expenses",
                    type: "nvarchar(max)",
                    nullable: false,
                    defaultValue: "");

                migrationBuilder.AddColumn<string>(
                    name: "PaymentMethod",
                    table: "Expenses",
                    type: "nvarchar(max)",
                    nullable: false,
                    defaultValue: "");
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.DropColumn(name: "VendorName", table: "Expenses");
                migrationBuilder.DropColumn(name: "PaymentMethod", table: "Expenses");
            }
        }
    }
}

