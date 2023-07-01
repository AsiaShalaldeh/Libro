using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class AddOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "0836c658-57c9-4ff7-8101-b681b82f7098");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "62f791b7-2e7d-47f7-8780-6f30bfb8bf62");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "fd1f4c48-daa1-43d5-8114-93a258651394");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "eb8cf27c-d9a9-4ee4-9693-34aeb679d996", "b0b41d90-2d57-43f3-8e31-6cf7bd885933" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
