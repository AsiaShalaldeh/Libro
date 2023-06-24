using Libro.Domain.Entities;
using Libro.Domain.Models;

namespace Libro.Domain.Interfaces.IServices
{
    public interface ITransactionService
    {
        Task<Reservation> ReserveBookAsync(Book book, Patron patron);
        Task<Checkout> CheckoutBookAsync(Book book, Patron patron);
        Task<ReturnResponseModel> ReturnBookAsync(Book book, Patron patron);
        Checkout GetActiveTransaction(string ISBN, int patronId);
        Task<IEnumerable<Book>> GetOverdueBooksAsync();
        Task<IEnumerable<Book>> GetBorrowedBooksAsync();
        Task<Book> GetBorrowedBookByIdAsync(string ISBN);
        Task<IEnumerable<Checkout>> GetTransactionsByPatron(int patronId);
        public IEnumerable<Checkout> GetOverdueTransactionsAsync();
    }
}
