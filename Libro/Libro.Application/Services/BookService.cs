using AutoMapper;
using FluentValidation;
using Libro.Application.Validators;
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
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;


        public BookService(IBookRepository bookRepository, IMapper mapper, 
            IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<Book> GetBookByISBNAsync(string ISBN)
        {
            Book book = await _bookRepository.GetBookByISBNAsync(ISBN);
            return book;
        }
        public async Task<PaginatedResult<BookDto>> GetAllBooksAsync(int pageNumber, int pageSize)
        {
            var paginatedResult = await _bookRepository.GetAllBooksAsync(pageNumber, pageSize);

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

            return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
        }
        public async Task<PaginatedResult<BookDto>> SearchBooksAsync(string title, string author,
            string genre, int pageNumber, int pageSize)
        {
            var paginatedResult = await _bookRepository.SearchBooksAsync(title, author, genre, pageNumber, pageSize);

            var bookDtos = _mapper.Map<IEnumerable<BookDto>>(paginatedResult.Items);

            return new PaginatedResult<BookDto>(bookDtos, paginatedResult.TotalCount, pageNumber, pageSize);
        }
        public async Task<BookDto> AddBookAsync(BookRequest bookDto)
        {
            Author author = await _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId);
            if (author == null)
            {
                throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
            }
            var book = _mapper.Map<Book>(bookDto);
            if (author.Books == null)
            {
                author.Books = new List<Book>();
            }
            await _bookRepository.AddBookAsync(book, author);
            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(string ISBN, BookRequest bookDto)
        {
            var existingBook = await _bookRepository.GetBookByISBNAsync(ISBN);
            if (existingBook == null)
            {
                throw new ResourceNotFoundException("Book", "ID", ISBN);
            }
            if (bookDto.AuthorId != null)
            {
                Author author = await _authorRepository.GetAuthorByIdAsync(bookDto.AuthorId);
                if (author == null)
                {
                    throw new ResourceNotFoundException("Author", "ID", bookDto.AuthorId.ToString());
                }
                existingBook.Author = author;
            }
            await _bookRepository.UpdateBookAsync(existingBook);
        }

        public async Task RemoveBookAsync(string ISBN)
        {
            var existingBook = await _bookRepository.GetBookByISBNAsync(ISBN);
            if (existingBook == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", ISBN);
            }

            await _bookRepository.DeleteBookAsync(existingBook);
        }

        public async Task<IEnumerable<Book>> GetBooksByGenres(IEnumerable<Genre> genres)
        {
            return await _bookRepository.GetBooksByGenres(genres);
        }
    }

}
