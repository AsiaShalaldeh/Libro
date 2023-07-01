using Libro.Domain.Entities;
using Libro.Infrastructure.Data;

namespace Libro.Tests.MockData
{
    public static class ReadingListMockData
    {
        public static void InitializeTestData(LibroDbContext dbContext)
        {

            // Add sample data
            var readingList1 = new ReadingList { ReadingListId = 1, PatronId = "123", Name = "Favorites" };
            var readingList2 = new ReadingList { ReadingListId = 2, PatronId = "123", Name = "To Read"};

            var bookList1 = new BookList { BookId = "9780987654321", ReadingListId = 1 };
            var bookList2 = new BookList { BookId = "9781234567890", ReadingListId = 1 };
            var bookList3 = new BookList { BookId = "9780987654321", ReadingListId = 2 };

            var book1 = new Book { ISBN = "9780987654321", Title = "Book1" };
            var book2 = new Book { ISBN = "9781234567890", Title = "Book2" };

            dbContext.ReadingLists.AddRange(readingList1, readingList2);
            dbContext.BookLists.AddRange(bookList1, bookList2, bookList3);
            dbContext.Books.AddRange(book1, book2);

            dbContext.SaveChanges();
        }
    }
}
