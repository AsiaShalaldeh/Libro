using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class AddQueueTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookQueues",
                columns: table => new
                {
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatronId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QueuePosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookQueues", x => new { x.BookId, x.PatronId });
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "e4baa6db-343d-40af-9116-f6a4e00626bb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "4e259e65-71df-4aef-9cf8-a667f3e0cd8a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "4d0a6b16-3555-447f-acfd-e29939dc2717");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "8d81c218-1cb8-449e-9ba3-6b98179b9873", "bbb3fc0f-6256-4ddb-8491-d9a4d250a046" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookQueues");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "176ba64f-16ee-404a-8b23-4c65410916bd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "c9bae31b-d5fd-4ed1-a02b-d354894f919d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "5341ee4c-6db1-4d3a-afba-6b607c8482c5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "c9e6a899-e83f-440a-a2eb-c91feb2c0184", "77950ead-de71-44e5-ba49-c6ecff3a0eb0" });
        }
    }
}
