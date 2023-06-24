using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class SeedUsersData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "12804f00-c9f8-4d2d-9e5f-eb8aa485368f", "b74ddd14-6340-4840-95c2-db12554843e5" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "652e8ec1-bc12-4511-b5d3-4a86f0f8515b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "56b4f6e6-d7a4-4b75-94a7-c85357bad7f0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "59d208d6-8b6c-4854-a04b-87b1e2cd4af5");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "3ff5ce26-a23a-4278-a442-fb87a687045e", "d85cd344-14e5-4bd7-aa68-ebd8fa460467" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "7ca524dd-8174-42f6-807e-3021c2a92d64");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "b81ac237-79fb-440e-b40e-28b9deb283d3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "697f70c2-2d90-4782-99b5-74794c1fb4d9");

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "12804f00-c9f8-4d2d-9e5f-eb8aa485368f", "b74ddd14-6340-4840-95c2-db12554843e5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "b4d188f9-214a-43c6-82ce-a7c019d24c8e", "02209acb-f190-4cb6-81b4-18d2f7d6c534" });
        }
    }
}
