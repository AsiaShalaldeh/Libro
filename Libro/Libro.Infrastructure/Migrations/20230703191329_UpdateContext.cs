using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class UpdateContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookList_Books_BookId",
                table: "BookList");

            migrationBuilder.DropForeignKey(
                name: "FK_BookList_ReadingLists_ReadingListId",
                table: "BookList");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookList",
                table: "BookList");

            migrationBuilder.RenameTable(
                name: "BookList",
                newName: "BookLists");

            migrationBuilder.RenameIndex(
                name: "IX_BookList_BookId",
                table: "BookLists",
                newName: "IX_BookLists_BookId");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookLists",
                table: "BookLists",
                columns: new[] { "ReadingListId", "BookId" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12804f00-c9f8-4d2d-9e5f-eb8aa485368f",
                column: "ConcurrencyStamp",
                value: "c68d6337-af80-4b67-83fb-dd9ee2aa9fea");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c",
                column: "ConcurrencyStamp",
                value: "66dbe67d-de47-479b-8311-2c83242d955b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6f004ec-feb9-40bf-9e52-09a563fb2fb9",
                column: "ConcurrencyStamp",
                value: "73332ff3-d2a7-462a-a51d-9310bfbe02cf");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "1db6ab82-cd78-4930-a5d4-1fa90dc1d1ea", "e2c2463b-a9a2-4dd7-b858-32bc19583a76" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookLists_Books_BookId",
                table: "BookLists",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookLists_ReadingLists_ReadingListId",
                table: "BookLists",
                column: "ReadingListId",
                principalTable: "ReadingLists",
                principalColumn: "ReadingListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookLists_Books_BookId",
                table: "BookLists");

            migrationBuilder.DropForeignKey(
                name: "FK_BookLists_ReadingLists_ReadingListId",
                table: "BookLists");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookLists",
                table: "BookLists");

            migrationBuilder.RenameTable(
                name: "BookLists",
                newName: "BookList");

            migrationBuilder.RenameIndex(
                name: "IX_BookLists_BookId",
                table: "BookList",
                newName: "IX_BookList_BookId");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookList",
                table: "BookList",
                columns: new[] { "ReadingListId", "BookId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_BookList_Books_BookId",
                table: "BookList",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookList_ReadingLists_ReadingListId",
                table: "BookList",
                column: "ReadingListId",
                principalTable: "ReadingLists",
                principalColumn: "ReadingListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
