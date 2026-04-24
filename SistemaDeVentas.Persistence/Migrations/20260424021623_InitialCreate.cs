using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeVentas.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DimCategory",
                columns: table => new
                {
                    CategoryKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimCategory", x => x.CategoryKey);
                });

            migrationBuilder.CreateTable(
                name: "DimDate",
                columns: table => new
                {
                    DateKey = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Quarter = table.Column<byte>(type: "tinyint", nullable: false),
                    Month = table.Column<byte>(type: "tinyint", nullable: false),
                    MonthName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DayOfMonth = table.Column<byte>(type: "tinyint", nullable: false),
                    DayName = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimDate", x => x.DateKey);
                });

            migrationBuilder.CreateTable(
                name: "DimLocation",
                columns: table => new
                {
                    LocationKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimLocation", x => x.LocationKey);
                });

            migrationBuilder.CreateTable(
                name: "DimOrderStatus",
                columns: table => new
                {
                    StatusKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimOrderStatus", x => x.StatusKey);
                });

            migrationBuilder.CreateTable(
                name: "DimProduct",
                columns: table => new
                {
                    ProductKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID_NaturalKey = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CategoryKey = table.Column<int>(type: "int", nullable: true),
                    ListPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimProduct", x => x.ProductKey);
                    table.ForeignKey(
                        name: "FK_DimProduct_DimCategory_CategoryKey",
                        column: x => x.CategoryKey,
                        principalTable: "DimCategory",
                        principalColumn: "CategoryKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimCustomer",
                columns: table => new
                {
                    CustomerKey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID_NaturalKey = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationKey = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimCustomer", x => x.CustomerKey);
                    table.ForeignKey(
                        name: "FK_DimCustomer_DimLocation_LocationKey",
                        column: x => x.LocationKey,
                        principalTable: "DimLocation",
                        principalColumn: "LocationKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactSales",
                columns: table => new
                {
                    SalesKey = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateKey = table.Column<int>(type: "int", nullable: false),
                    CustomerKey = table.Column<int>(type: "int", nullable: false),
                    ProductKey = table.Column<int>(type: "int", nullable: false),
                    LocationKey = table.Column<int>(type: "int", nullable: true),
                    StatusKey = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SalesAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactSales", x => x.SalesKey);
                    table.ForeignKey(
                        name: "FK_FactSales_DimCustomer_CustomerKey",
                        column: x => x.CustomerKey,
                        principalTable: "DimCustomer",
                        principalColumn: "CustomerKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactSales_DimDate_DateKey",
                        column: x => x.DateKey,
                        principalTable: "DimDate",
                        principalColumn: "DateKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactSales_DimLocation_LocationKey",
                        column: x => x.LocationKey,
                        principalTable: "DimLocation",
                        principalColumn: "LocationKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactSales_DimOrderStatus_StatusKey",
                        column: x => x.StatusKey,
                        principalTable: "DimOrderStatus",
                        principalColumn: "StatusKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FactSales_DimProduct_ProductKey",
                        column: x => x.ProductKey,
                        principalTable: "DimProduct",
                        principalColumn: "ProductKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimCategory_CategoryName",
                table: "DimCategory",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimCustomer_CustomerID_NaturalKey",
                table: "DimCustomer",
                column: "CustomerID_NaturalKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimCustomer_LocationKey",
                table: "DimCustomer",
                column: "LocationKey");

            migrationBuilder.CreateIndex(
                name: "IX_DimLocation_Country_City",
                table: "DimLocation",
                columns: new[] { "Country", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimOrderStatus_StatusName",
                table: "DimOrderStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DimProduct_CategoryKey",
                table: "DimProduct",
                column: "CategoryKey");

            migrationBuilder.CreateIndex(
                name: "IX_DimProduct_ProductID_NaturalKey",
                table: "DimProduct",
                column: "ProductID_NaturalKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FactSales_CustomerKey",
                table: "FactSales",
                column: "CustomerKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactSales_DateKey",
                table: "FactSales",
                column: "DateKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactSales_LocationKey",
                table: "FactSales",
                column: "LocationKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactSales_ProductKey",
                table: "FactSales",
                column: "ProductKey");

            migrationBuilder.CreateIndex(
                name: "IX_FactSales_StatusKey",
                table: "FactSales",
                column: "StatusKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactSales");

            migrationBuilder.DropTable(
                name: "DimCustomer");

            migrationBuilder.DropTable(
                name: "DimDate");

            migrationBuilder.DropTable(
                name: "DimOrderStatus");

            migrationBuilder.DropTable(
                name: "DimProduct");

            migrationBuilder.DropTable(
                name: "DimLocation");

            migrationBuilder.DropTable(
                name: "DimCategory");
        }
    }
}
