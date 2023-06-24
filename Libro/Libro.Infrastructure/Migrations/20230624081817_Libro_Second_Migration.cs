using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class Libro_Second_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Books_BookId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Patrons_PatronId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Patrons_PatronId1",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PatronId1",
                table: "Transactions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ea6c8c9-e8d0-416a-a521-2ac713356f1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3f068631-f9ce-4b18-b85b-8dbfedf5f2fb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5cd66edb-5619-4806-8312-5d2da05bf3c3");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PatronId1",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Checkouts");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Checkouts",
                newName: "CheckoutDate");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Checkouts",
                newName: "CheckoutId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_PatronId",
                table: "Checkouts",
                newName: "IX_Checkouts_PatronId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_BookId",
                table: "Checkouts",
                newName: "IX_Checkouts_BookId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFee",
                table: "Checkouts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Checkouts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReturned",
                table: "Checkouts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Checkouts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkouts",
                table: "Checkouts",
                column: "CheckoutId");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PatronId = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Patrons_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patrons",
                        principalColumn: "PatronId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookId",
                table: "Reservations",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PatronId",
                table: "Reservations",
                column: "PatronId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Books_BookId",
                table: "Checkouts",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Patrons_PatronId",
                table: "Checkouts",
                column: "PatronId",
                principalTable: "Patrons",
                principalColumn: "PatronId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Books_BookId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Patrons_PatronId",
                table: "Checkouts");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkouts",
                table: "Checkouts");

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

            migrationBuilder.RenameTable(
                name: "Checkouts",
                newName: "Transactions");

            migrationBuilder.RenameColumn(
                name: "CheckoutDate",
                table: "Transactions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "CheckoutId",
                table: "Transactions",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Checkouts_PatronId",
                table: "Transactions",
                newName: "IX_Transactions_PatronId");

            migrationBuilder.RenameIndex(
                name: "IX_Checkouts_BookId",
                table: "Transactions",
                newName: "IX_Transactions_BookId");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFee",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnDate",
                table: "Transactions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReturned",
                table: "Transactions",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Transactions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PatronId1",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "TransactionId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0ea6c8c9-e8d0-416a-a521-2ac713356f1d", "f59a4e6e-27fc-411c-a1f1-4f1a6b1ba4a4", "Librarian", "LIBRARIAN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3f068631-f9ce-4b18-b85b-8dbfedf5f2fb", "c864405f-18b9-4aac-b3f8-89824eb8c999", "Patron", "PATRON" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5cd66edb-5619-4806-8312-5d2da05bf3c3", "debf862a-0a47-46be-ac79-065b10497564", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PatronId1",
                table: "Transactions",
                column: "PatronId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Books_BookId",
                table: "Transactions",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Patrons_PatronId",
                table: "Transactions",
                column: "PatronId",
                principalTable: "Patrons",
                principalColumn: "PatronId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Patrons_PatronId1",
                table: "Transactions",
                column: "PatronId1",
                principalTable: "Patrons",
                principalColumn: "PatronId");
        }
    }
}
