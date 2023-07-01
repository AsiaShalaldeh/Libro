using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class TransactionMockData
    {
        public static void InitializeTestData(LibroDbContext dbContext)
        {
            var book1 = new Book { ISBN = "1", Title = "Book 1" };
            var book2 = new Book { ISBN = "2", Title = "Book 2" };

            var patron1 = new Patron { PatronId = "1", Name = "Patron 1", Email = "patron1@gmail.com" };
            var patron2 = new Patron { PatronId = "2", Name = "Patron 2", Email = "patron2@gmail.com"};

            var reservation1 = new Reservation
            {
                ReservationId = "1",
                BookId = book1.ISBN,
                PatronId = patron1.PatronId,
                ReservationDate = DateTime.Now.AddDays(2)
            };

            var reservation2 = new Reservation
            {
                ReservationId = "2",
                BookId = book2.ISBN,
                PatronId = patron1.PatronId,
                ReservationDate = DateTime.Now.AddDays(1)
            };

            var checkout1 = new Checkout
            {
                CheckoutId = "1",
                BookId = book1.ISBN,
                PatronId = patron1.PatronId,
                CheckoutDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false,
                ReturnDate = DateTime.MinValue,
                TotalFee = 0
            };

            var checkout2 = new Checkout
            {
                CheckoutId = "2",
                BookId = book2.ISBN,
                PatronId = patron2.PatronId,
                CheckoutDate = DateTime.Now.AddDays(-20),
                DueDate = DateTime.Now.AddDays(-6),
                IsReturned = false,
                ReturnDate = DateTime.Now.AddDays(3),
                TotalFee = 0
            };

            dbContext.Books.AddRange(book1, book2);
            dbContext.Patrons.AddRange(patron1, patron2);
            dbContext.Reservations.AddRange(reservation1, reservation2);
            dbContext.Checkouts.AddRange(checkout1, checkout2);
            dbContext.SaveChanges();
        }
    }
}
