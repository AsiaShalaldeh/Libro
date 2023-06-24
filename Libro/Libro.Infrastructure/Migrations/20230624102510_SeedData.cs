using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Infrastructure.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[,]
                {
                    { "12804f00-c9f8-4d2d-9e5f-eb8aa485368f", "3609c6d9-fb9e-4aff-97e2-fe8fb8add872", "Administrator", "ADMINISTRATOR" },
                    { "e1b13948-d0d2-4e4c-b706-8b70a99c8e6c", "8e621667-bec5-4236-8fd3-c0e12ecc8855", "Librarian", "LIBRARIAN" },
                    { "e6f004ec-feb9-40bf-9e52-09a563fb2fb9", "2750fcd9-ae76-418b-b327-d23c00cca0b4", "Patron", "PATRON" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "AuthorId", "Name" },
                values: new object[,]
                {
                    { 1, "John Green" },
                    { 2, "J.K. Rowling" },
                    { 3, "Stephen King" },
                    { 4, "Agatha Christie" },
                    { 5, "Haruki Murakami" }
                });

            migrationBuilder.InsertData(
                table: "Librarians",
                columns: new[] { "LibrarianId", "Name" },
                values: new object[,]
                {
                    { 1, "John Smith" },
                    { 2, "Emily Johnson" },
                    { 3, "Michael Davis" },
                    { 4, "Sarah Wilson" },
                    { 5, "David Thompson" }
                });

            migrationBuilder.InsertData(
                table: "Patrons",
                columns: new[] { "PatronId", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "johndoe@gmail.com", "John Doe" },
                    { 2, "janesmith@gmail.com", "Jane Smith" },
                    { 3, "michaeljohnson@gmail.com", "Michael Johnson" },
                    { 4, "emilydavis@gmail.com", "Emily Davis" },
                    { 5, "danielwilson@gmail.com", "Daniel Wilson" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "ISBN", "AuthorId", "Genre", "IsAvailable", "PublicationDate", "Title" },
                values: new object[,]
                {
                    { "9780061122415", 4, 8, true, new DateTime(1960, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "To Kill a Mockingbird" },
                    { "9780143127550", 3, 5, true, new DateTime(1949, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "1984" },
                    { "9780545010221", 2, 3, true, new DateTime(1997, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Harry Potter and the Sorcerer's Stone" },
                    { "9780743273565", 5, 2, true, new DateTime(2003, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Da Vinci Code" },
                    { "9781451673319", 1, 0, true, new DateTime(1925, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Great Gatsby" }
                });

            migrationBuilder.InsertData(
                table: "ReadingLists",
                columns: new[] { "ReadingListId", "Name", "PatronId" },
                values: new object[,]
                {
                    { 1, "Favorites", 1 },
                    { 2, "To Read", 1 },
                    { 3, "Classics", 2 },
                    { 4, "Mystery", 3 },
                    { 5, "Sci-Fi", 4 }
                });

            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "CheckoutId", "BookId", "CheckoutDate", "DueDate", "IsReturned", "PatronId", "ReturnDate", "TotalFee" },
                values: new object[,]
                {
                    { "1", "9780143127550", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0m },
                    { "2", "9781451673319", new DateTime(2023, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, new DateTime(2023, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1.99m },
                    { "3", "9780061122415", new DateTime(2023, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0m },
                    { "4", "9780743273565", new DateTime(2023, 6, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 4, new DateTime(2023, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.99m },
                    { "5", "9780545010221", new DateTime(2023, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.0m }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "ReservationId", "BookId", "PatronId", "ReservationDate" },
                values: new object[,]
                {
                    { "1", "9780545010221", 1, new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "2", "9780743273565", 2, new DateTime(2023, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "3", "9780061122415", 3, new DateTime(2023, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "4", "9781451673319", 4, new DateTime(2023, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { "5", "9780545010221", 5, new DateTime(2023, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "BookId", "Comment", "PatronId", "Rating" },
                values: new object[,]
                {
                    { 1, "9780545010221", "Great book!", 1, 4 },
                    { 2, "9780545010221", "Interesting read.", 2, 3 },
                    { 3, "9781451673319", "Highly recommended!", 3, 5 },
                    { 4, "9781451673319", "Disappointing.", 4, 2 },
                    { 5, "9780743273565", "Loved it!", 5, 5 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "CheckoutId",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "CheckoutId",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "CheckoutId",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "CheckoutId",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "Checkouts",
                keyColumn: "CheckoutId",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "Librarians",
                keyColumn: "LibrarianId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Librarians",
                keyColumn: "LibrarianId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Librarians",
                keyColumn: "LibrarianId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Librarians",
                keyColumn: "LibrarianId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Librarians",
                keyColumn: "LibrarianId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ReadingLists",
                keyColumn: "ReadingListId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ReadingLists",
                keyColumn: "ReadingListId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ReadingLists",
                keyColumn: "ReadingListId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ReadingLists",
                keyColumn: "ReadingListId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ReadingLists",
                keyColumn: "ReadingListId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "ReservationId",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "9780061122415");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "9780143127550");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "9780545010221");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "9780743273565");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "ISBN",
                keyValue: "9781451673319");

            migrationBuilder.DeleteData(
                table: "Patrons",
                keyColumn: "PatronId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patrons",
                keyColumn: "PatronId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patrons",
                keyColumn: "PatronId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patrons",
                keyColumn: "PatronId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Patrons",
                keyColumn: "PatronId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "AuthorId",
                keyValue: 5);

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
    }
}
