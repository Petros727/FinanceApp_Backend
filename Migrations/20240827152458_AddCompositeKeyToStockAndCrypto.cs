using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeKeyToStockAndCrypto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cryptos",
                table: "Cryptos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                columns: new[] { "Symbol", "Date" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cryptos",
                table: "Cryptos",
                columns: new[] { "Symbol", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cryptos",
                table: "Cryptos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Stocks",
                table: "Stocks",
                column: "Symbol");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cryptos",
                table: "Cryptos",
                column: "Symbol");
        }
    }
}
