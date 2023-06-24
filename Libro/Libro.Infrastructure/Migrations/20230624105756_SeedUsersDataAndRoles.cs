using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class SeedUsersDataAndRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1cb2121d-77a2-4eca-bed3-230bb513a800", "156f6971-a273-4b18-8210-5aed44edcd8f", "Administrator", "ADMINISTRATOR" },
                    { "ae4d945d-3889-4a95-9793-b1c695fc4831", "386b3e3f-59bd-478f-994c-0e88fd5f6292", "Patron", "PATRON" },
                    { "db10ec09-53e7-4613-b8a3-2ca45d710ded", "704a21cd-6ed5-45a8-b512-69af03fe6d7d", "Librarian", "LIBRARIAN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b74ddd14-6340-4840-95c2-db12554843e5", 0, "d4046e6b-63c8-4197-9291-5850700509ff", "admin@gmail.com", false, false, null, null, null, null, null, false, "651708e3-f44d-4deb-8e9d-8e937eb19391", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1cb2121d-77a2-4eca-bed3-230bb513a800", "b74ddd14-6340-4840-95c2-db12554843e5" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1cb2121d-77a2-4eca-bed3-230bb513a800");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ae4d945d-3889-4a95-9793-b1c695fc4831");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db10ec09-53e7-4613-b8a3-2ca45d710ded");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "Admin", "b74ddd14-6340-4840-95c2-db12554843e5" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "12804f00-c9f8-4d2d-9e5f-eb8aa485368f", "3609c6d9-fb9e-4aff-97e2-fe8fb8add872", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c", "8e621667-bec5-4236-8fd3-c0e12ecc8855", "Librarian", "LIBRARIAN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e6f004ec-feb9-40bf-9e52-09a563fb2fb9", "2750fcd9-ae76-418b-b327-d23c00cca0b4", "Patron", "PATRON" });
        }
    }
}
