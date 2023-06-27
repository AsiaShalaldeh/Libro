using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Libro.Infrastructure.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patron>().HasData(
                new Patron { PatronId = "1", Name = "John Doe", Email = "johndoe@gmail.com" },
                new Patron { PatronId = "2", Name = "Jane Smith", Email = "janesmith@gmail.com" },
                new Patron { PatronId = "3", Name = "Michael Johnson", Email = "michaeljohnson@gmail.com" },
                new Patron { PatronId = "4", Name = "Emily Davis", Email = "emilydavis@gmail.com" },
                new Patron { PatronId = "5", Name = "Daniel Wilson", Email = "danielwilson@gmail.com" }
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
                    DueDate = new DateTime(2023, 6, 12),
                    IsReturned = true,
                    ReturnDate = new DateTime(2023, 6, 10),
                    TotalFee = 1.99m
                },
                new Checkout
                {
                    CheckoutId = "3",
                    BookId = "9780061122415",
                    PatronId = "3",
                    CheckoutDate = new DateTime(2023, 6, 10),
                    DueDate = new DateTime(2023, 7, 1),
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
                new Librarian { LibrarianId = "1", Name = "John Smith" },
                new Librarian { LibrarianId = "2", Name = "Emily Johnson" },
                new Librarian { LibrarianId = "3", Name = "Michael Davis" },
                new Librarian { LibrarianId = "4", Name = "Sarah Wilson" },
                new Librarian { LibrarianId = "5", Name = "David Thompson" }
            );
            modelBuilder.Entity<ReadingList>().HasData(
                new ReadingList { ReadingListId = 1, Name = "Favorites", PatronId = "1" },
                new ReadingList { ReadingListId = 2, Name = "To Read", PatronId = "1" },
                new ReadingList { ReadingListId = 3, Name = "Classics", PatronId = "2" },
                new ReadingList { ReadingListId = 4, Name = "Mystery", PatronId = "3" },
                new ReadingList { ReadingListId = 5, Name = "Sci-Fi", PatronId = "4" }
            );
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ReservationId = "1",
                    BookId = "9780545010221",
                    PatronId = "1",
                    ReservationDate = new DateTime(2023, 6, 15)
                },
                new Reservation
                {
                    ReservationId = "2",
                    BookId = "9780743273565",
                    PatronId = "2",
                    ReservationDate = new DateTime(2023, 6, 16)
                },
                new Reservation
                {
                    ReservationId = "3",
                    BookId = "9780061122415",
                    PatronId = "3",
                    ReservationDate = new DateTime(2023, 6, 17)
                },
                new Reservation
                {
                    ReservationId = "4",
                    BookId = "9781451673319",
                    PatronId = "4",
                    ReservationDate = new DateTime(2023, 6, 18)
                },
                new Reservation
                {
                    ReservationId = "5",
                    BookId = "9780545010221",
                    PatronId = "5",
                    ReservationDate = new DateTime(2023, 6, 19)
                }
            );
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ReviewId = 1,
                    PatronId = "1",
                    BookId = "9780545010221",
                    Rating = 4,
                    Comment = "Great book!"
                },
                new Review
                {
                    ReviewId = 2,
                    PatronId = "2",
                    BookId = "9780545010221",
                    Rating = 3,
                    Comment = "Interesting read."
                },
                new Review
                {
                    ReviewId = 3,
                    PatronId = "3",
                    BookId = "9781451673319",
                    Rating = 5,
                    Comment = "Highly recommended!"
                },
                new Review
                {
                    ReviewId = 4,
                    PatronId = "4",
                    BookId = "9781451673319",
                    Rating = 2,
                    Comment = "Disappointing."
                },
                new Review
                {
                    ReviewId = 5,
                    PatronId = "1",
                    BookId = "9780743273565",
                    Rating = 5,
                    Comment = "Loved it!"
                }
            );
            IdentityUser user = new IdentityUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                Email = "admin@gmail.com",
            };
            PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            passwordHasher.HashPassword(user, "Admin*123");

            modelBuilder.Entity<IdentityUser>().HasData(user);

            //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            //new IdentityUserRole<string> { UserId = "b74ddd14-6340-4840-95c2-db12554843e5", RoleId = "12804f00-c9f8-4d2d-9e5f-eb8aa485368f" }
            //);
        }
    }
}
