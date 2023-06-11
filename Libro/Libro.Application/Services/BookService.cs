using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<Book> GetBookByIdAsync(string ISBN)
        {
            return await _bookRepository.GetByIdAsync(ISBN);
        }

        public async Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            var paginatedResult = await _bookRepository.SearchAsync(title, author, genre, pageNumber, pageSize);

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

            return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            var paginatedResult = await _bookRepository.GetAllAsync(pageNumber, pageSize);

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

            return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
        }
        public async Task<Transaction> ReserveBookAsync(string isbn, int patronId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            var transaction = await _bookRepository.ReserveAsync(isbn, patronId);
            return transaction; 
        }

        public async Task<Transaction> CheckoutBookAsync(string isbn, int patronId, int librarianId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            if (librarianId <= 0)
                throw new ArgumentException("Invalid librarian ID.", nameof(librarianId));

            var transaction = await _bookRepository.CheckoutAsync(isbn, patronId, librarianId);
            return transaction;
        }

        public async Task<Transaction> ReturnBookAsync(string isbn, int patronId)
        {
            if (string.IsNullOrEmpty(isbn))
                throw new ArgumentException("ISBN is required.", nameof(isbn));

            if (patronId <= 0)
                throw new ArgumentException("Invalid patron ID.", nameof(patronId));

            var transaction = await _bookRepository.ReturnAsync(isbn, patronId);
            return transaction;
        }
        public async Task<IEnumerable<Book>> GetOverdueBooksAsync()
        {
            var overdueBookIds = await _bookRepository.GetOverdueBooksAsync();
            var overdueBooks = new List<Book>();

            foreach (var bookId in overdueBookIds)
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book != null)
                {
                    overdueBooks.Add(book);
                }
            }

            return overdueBooks;
        }
        public async Task<IEnumerable<Book>> GetBorrowedBooksAsync()
        {
            var borrowedBookIds = await _bookRepository.GetBorrowedBooksAsync();
            var borrowedBooks = new List<Book>();

            foreach (var bookId in borrowedBookIds)
            {
                var book = await _bookRepository.GetByIdAsync(bookId);
                if (book != null)
                {
                    borrowedBooks.Add(book);
                }
            }
            return borrowedBooks;
        }
        public async Task<Book> GetBorrowedBookByIdAsync(string ISBN)
        {
            if (string.IsNullOrEmpty(ISBN))
                throw new ArgumentException("ISBN is required.", nameof(ISBN));
            var borrowedBookId = await _bookRepository.GetBorrowedBookByIdAsync(ISBN);
            if (string.IsNullOrEmpty(borrowedBookId))
                return null;
            var borrowedBook = await _bookRepository.GetByIdAsync(borrowedBookId);
            return borrowedBook;
        }



    }

}
