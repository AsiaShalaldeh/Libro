using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IAuthorRepository _authorRepository;

        public BookService(IBookRepository bookRepository, IMapper mapper, 
            IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _authorRepository = authorRepository;
        }

        public async Task<Book> GetBookByIdAsync(string ISBN)
        {
            Book book = await _bookRepository.GetByIdAsync(ISBN);
            //if (book != null)
            //{
            //    throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            //}
            return book;
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
        public async Task AddBookAsync(RequestBookDto bookDto)
        {
            var author = _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId);
            if (author == null)
            {
                throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
            }

            var book = _mapper.Map<Book>(bookDto);
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(string bookId, RequestBookDto bookDto)
        {
            var existingBook = await _bookRepository.GetByIdAsync(bookId);
            if (existingBook == null)
            {
                throw new ResourceNotFoundException("Book", "ID", bookId);
            }

            var author = _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId);
            if (author == null)
            {
                throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
            }
            if (string.IsNullOrEmpty(bookDto.ISBN))
            {
                existingBook.ISBN = bookDto.ISBN;
            }
            if (string.IsNullOrEmpty(bookDto.Title))
            {
                existingBook.Title = bookDto.Title;
            }
            if (bookDto.PublicationDate != null)
            {
                existingBook.PublicationDate = bookDto.PublicationDate;
            }
            if (bookDto.Genre != null)
            {
                existingBook.Genre = (Genre)Enum.Parse(typeof(Genre), bookDto.Genre);
            }
            if (bookDto.IsAvailable != null)
            {
                existingBook.IsAvailable = bookDto.IsAvailable;
            }
            existingBook.Author = author;
            await _bookRepository.UpdateAsync(existingBook);
        }

        public async Task RemoveBookAsync(string bookId)
        {
            var existingBook = await _bookRepository.GetByIdAsync(bookId);
            if (existingBook == null)
            {
                throw new ResourceNotFoundException($"Book", "ISBN", bookId);
            }

            await _bookRepository.DeleteAsync(bookId);
        }

        public async Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres)
        {
            return await _bookRepository.GetBooksByGenres(genres);
        }
    }

}
