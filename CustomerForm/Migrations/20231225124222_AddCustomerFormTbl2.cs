using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerForm.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerFormTbl2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Active",
                table: "CustomerForm",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerResultId",
                table: "CustomerForm",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "CustomerForm");

            migrationBuilder.DropColumn(
                name: "CustomerResultId",
                table: "CustomerForm");
        }
    }
}
