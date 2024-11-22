using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyERP.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    IntProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StrProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumStock = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.IntProductId);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    IntOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IntProductId = table.Column<int>(type: "int", nullable: false),
                    StrCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DtOrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductIntProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.IntOrderId);
                    table.ForeignKey(
                        name: "FK_orders_products_ProductIntProductId",
                        column: x => x.ProductIntProductId,
                        principalTable: "products",
                        principalColumn: "IntProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_ProductIntProductId",
                table: "orders",
                column: "ProductIntProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
