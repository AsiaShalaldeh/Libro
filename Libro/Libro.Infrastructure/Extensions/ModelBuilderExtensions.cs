using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patron>().HasData(
                new Patron { PatronId = "b74ddd14-6340-4840-95c2-db12964843e5", Name = "John Doe", Email = "johndoe@gmail.com" },
                new Patron { PatronId = "b74ddd14-6340-4840-95c2-db12523843e5", Name = "Jane Smith", Email = "janesmith@gmail.com" },
                new Patron { PatronId = "b74ddd14-6340-4840-95c2-db12590843e5", Name = "Michael Johnson", Email = "michaeljohnson@gmail.com" },
                new Patron { PatronId = "b74ddd14-6340-4840-95c2-db12512843e5", Name = "Emily Davis", Email = "emilydavis@gmail.com" },
                new Patron { PatronId = "b74ddd14-6340-4840-95c2-db12547843e5", Name = "Daniel Wilson", Email = "danielwilson@gmail.com" }
            );
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, Name = "John Green" },
                new Author { AuthorId = 2, Name = "J.K. Rowling" },
                new Author { AuthorId = 3, Name = "Stephen King" },
                new Author { AuthorId = 4, Name = "Agatha Christie" },
                new Author { AuthorId = 5, Name = "Haruki Murakami" }
            );
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    ISBN = "9780545010221",
                    Title = "Harry Potter and the Sorcerer's Stone",
                    PublicationDate = new DateTime(1997, 6, 26),
                    Genre = Genre.Fantasy,
                    IsAvailable = true,
                    AuthorId = 2 
                },
                new Book
                {
                    ISBN = "9780743273565",
                    Title = "The Da Vinci Code",
                    PublicationDate = new DateTime(2003, 3, 18),
                    Genre = Genre.ScienceFiction,
                    IsAvailable = true,
                    AuthorId = 5
                },
                new Book
                {
                    ISBN = "9780061122415",
                    Title = "To Kill a Mockingbird",
                    PublicationDate = new DateTime(1960, 7, 11),
                    Genre = Genre.Poetry,
                    IsAvailable = true,
                    AuthorId = 4
                },
                new Book
                {
                    ISBN = "9781451673319",
                    Title = "The Great Gatsby",
                    PublicationDate = new DateTime(1925, 4, 10),
                    Genre = Genre.Mystery,
                    IsAvailable = true,
                    AuthorId = 1
                },
                new Book
                {
                    ISBN = "9780143127550",
                    Title = "1984",
                    PublicationDate = new DateTime(1949, 6, 8),
                    Genre = Genre.History,
                    IsAvailable = true,
                    AuthorId = 3
                }
            );
            modelBuilder.Entity<Checkout>().HasData(
                new Checkout
                {
                    CheckoutId = "1",
                    BookId = "9780143127550",
                    PatronId = "1",
                    CheckoutDate = new DateTime(2023, 6, 1),
                    DueDate = new DateTime(2023, 6, 15),
                    IsReturned = false,
                    ReturnDate = DateTime.MinValue,
                    TotalFee = 0.0m
                },
                new Checkout
                {
                    CheckoutId = "2",
                    BookId = "9781451673319",
                    PatronId = "2",
                    CheckoutDate = new DateTime(2023, 6, 5),
                    DueDate = new DateTime(2023, 6, 19),
                    IsReturned = true,
                    ReturnDate = new DateTime(2023, 6, 18),
                    TotalFee = 1.99m
                },
                new Checkout
                {
                    CheckoutId = "3",
                    BookId = "9780061122415",
                    PatronId = "3",
                    CheckoutDate = new DateTime(2023, 6, 10),
                    DueDate = new DateTime(2023, 6, 24),
                    IsReturned = false,
                    ReturnDate = DateTime.MinValue,
                    TotalFee = 0.0m
                },
                new Checkout
                {
                    CheckoutId = "4",
                    BookId = "9780743273565",
                    PatronId = "4",
                    CheckoutDate = new DateTime(2023, 6, 2),
                    DueDate = new DateTime(2023, 6, 16),
                    IsReturned = true,
                    ReturnDate = new DateTime(2023, 6, 9),
                    TotalFee = 3.99m
                },
                new Checkout
                {
                    CheckoutId = "5",
                    BookId = "9780545010221",
                    PatronId = "5",
                    CheckoutDate = new DateTime(2023, 6, 8),
                    DueDate = new DateTime(2023, 6, 15),
                    IsReturned = false,
                    ReturnDate = DateTime.MinValue,
                    TotalFee = 0.0m
                }
            );
            modelBuilder.Entity<Librarian>().HasData(
                new Librarian { LibrarianId = Guid.NewGuid().ToString(), Name = "John Smith" },
                new Librarian { LibrarianId = Guid.NewGuid().ToString(), Name = "Emily Johnson" },
                new Librarian { LibrarianId = Guid.NewGuid().ToString(), Name = "Michael Davis" },
                new Librarian { LibrarianId = Guid.NewGuid().ToString(), Name = "Sarah Wilson" },
                new Librarian { LibrarianId = Guid.NewGuid().ToString(), Name = "David Thompson" }
            );
            modelBuilder.Entity<ReadingList>().HasData(
                new ReadingList { ReadingListId = 1, Name = "Favorites", PatronId = "b74ddd14-6340-4840-95c2-db12964843e5" },
                new ReadingList { ReadingListId = 2, Name = "To Read", PatronId = "b74ddd14-6340-4840-95c2-db12523843e5" },
                new ReadingList { ReadingListId = 3, Name = "Classics", PatronId = "b74ddd14-6340-4840-95c2-db12590843e5" },
                new ReadingList { ReadingListId = 4, Name = "Mystery", PatronId = "b74ddd14-6340-4840-95c2-db12512843e5" },
                new ReadingList { ReadingListId = 5, Name = "Sci-Fi", PatronId = "b74ddd14-6340-4840-95c2-db12547843e5" }
            );
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ReservationId = "a1c2-db12964843e5",
                    BookId = "9780545010221",
                    PatronId = "b74ddd14-6340-4840-95c2-db12964843e5",
                    ReservationDate = new DateTime(2023, 6, 15)
                },
                new Reservation
                {
                    ReservationId = "a2c2-db12964843e5",
                    BookId = "9780743273565",
                    PatronId = "b74ddd14-6340-4840-95c2-db12523843e5",
                    ReservationDate = new DateTime(2023, 6, 16)
                },
                new Reservation
                {
                    ReservationId = "a3c2-db12964843e5",
                    BookId = "9780061122415",
                    PatronId = "b74ddd14-6340-4840-95c2-db12590843e5",
                    ReservationDate = new DateTime(2023, 6, 17)
                },
                new Reservation
                {
                    ReservationId = "a4c2-db12964843e5",
                    BookId = "9781451673319",
                    PatronId = "b74ddd14-6340-4840-95c2-db12512843e5",
                    ReservationDate = new DateTime(2023, 6, 18)
                },
                new Reservation
                {
                    ReservationId = "a5c2-db12964843e5",
                    BookId = "9780545010221",
                    PatronId = "b74ddd14-6340-4840-95c2-db12547843e5",
                    ReservationDate = new DateTime(2023, 6, 19)
                }
            );
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ReviewId = 1,
                    PatronId = "b74ddd14-6340-4840-95c2-db12547843e5",
                    BookId = "9780545010221",
                    Rating = 4,
                    Comment = "Great book!"
                },
                new Review
                {
                    ReviewId = 2,
                    PatronId = "b74ddd14-6340-4840-95c2-db12512843e5",
                    BookId = "9780545010221",
                    Rating = 3,
                    Comment = "Interesting read."
                },
                new Review
                {
                    ReviewId = 3,
                    PatronId = "b74ddd14-6340-4840-95c2-db12590843e5",
                    BookId = "9781451673319",
                    Rating = 5,
                    Comment = "Highly recommended!"
                },
                new Review
                {
                    ReviewId = 4,
                    PatronId = "b74ddd14-6340-4840-95c2-db12523843e5",
                    BookId = "9781451673319",
                    Rating = 2,
                    Comment = "Disappointing."
                },
                new Review
                {
                    ReviewId = 5,
                    PatronId = "b74ddd14-6340-4840-95c2-db12964843e5",
                    BookId = "9780743273565",
                    Rating = 5,
                    Comment = "Loved it!"
                }
            );
            //IdentityUser user1 = new IdentityUser()
            //{
            //    Id = "b74ddd14-6340-4840-95c2-db12554843e5",
            //    UserName = "Admin",
            //    Email = "admin@gmail.com",
            //};
            //var passwordHasher = new PasswordHasher<IdentityUser>();
            //user1.PasswordHash = passwordHasher.HashPassword(user1, "Admin123");

            //modelBuilder.Entity<IdentityUser>().HasData(user1);

            var hasher = new PasswordHasher<IdentityUser>();

            var adminUser = new IdentityUser
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Admin123")
            };

            var librarianUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "LibrarianUser",
                NormalizedUserName = "LIBRARIANUSER",
                Email = "librarian@gmail.com",
                NormalizedEmail = "LIBRARIAN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Librarian123")
            };

            var patronUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "PatronUser",
                NormalizedUserName = "PATRONUSER",
                Email = "patron@gmail.com",
                NormalizedEmail = "PATRON@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Patron123")
            };

            var roleConfig = new RoleConfiguration();

            string adminRoleId = roleConfig.AdminRoleId;
            string librarianRoleId = roleConfig.LibrarianRoleId;
            string patronRoleId = roleConfig.PatronRoleId;

            modelBuilder.Entity<IdentityUser>().HasData(adminUser, librarianUser, patronUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = librarianUser.Id, RoleId = librarianRoleId },
                new IdentityUserRole<string> { UserId = patronUser.Id, RoleId = patronRoleId }
            );
        } 
        // I may the logic of adding Admin Role for the first user 
    }
}
