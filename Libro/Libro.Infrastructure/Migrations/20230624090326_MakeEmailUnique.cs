using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class MakeEmailUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4dfa55cb-6e8e-4d57-8202-c4c8d762eb20");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a23c954-4d14-499e-84b2-3f5aa54e4509");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f542930c-d7d2-4cb2-8bbf-473564c57002");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6aca3b36-c1a2-47a1-99c9-a8ad0ca70868", "468cf244-813d-4a87-a5b5-f3d39bfe2add", "Patron", "PATRON" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bd8f8b4b-c3b6-4081-b26a-feeb34fffee6", "0c49efe4-2bdb-4f69-9e5f-6752b6f3316a", "Librarian", "LIBRARIAN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f70cbc68-6596-41d1-8933-49f4a2aa1319", "aa9e1483-4c0c-4e57-a6f4-903ad0c30a16", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6aca3b36-c1a2-47a1-99c9-a8ad0ca70868");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd8f8b4b-c3b6-4081-b26a-feeb34fffee6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f70cbc68-6596-41d1-8933-49f4a2aa1319");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4dfa55cb-6e8e-4d57-8202-c4c8d762eb20", "6dd0c45a-22e7-4bf3-8c4e-cb5f0c40221d", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8a23c954-4d14-499e-84b2-3f5aa54e4509", "931835c5-0ad2-4c45-ae0d-d6f2e553b91a", "Patron", "PATRON" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "f542930c-d7d2-4cb2-8bbf-473564c57002", "f8dfede5-f1a3-41c9-a4c3-aa02da433efd", "Librarian", "LIBRARIAN" });
        }
    }
}
