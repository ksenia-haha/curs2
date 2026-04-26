using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExemplarPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesAndExemplars",
                table: "SalesAndExemplars");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SalesAndExemplars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Exemplars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesAndExemplars",
                table: "SalesAndExemplars",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAndExemplars_SaleId",
                table: "SalesAndExemplars",
                column: "SaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesAndExemplars",
                table: "SalesAndExemplars");

            migrationBuilder.DropIndex(
                name: "IX_SalesAndExemplars_SaleId",
                table: "SalesAndExemplars");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SalesAndExemplars");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Exemplars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesAndExemplars",
                table: "SalesAndExemplars",
                columns: new[] { "SaleId", "ExemplarId" });
        }
    }
}
