using AutoMapper;
using FluentValidation;
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
    }

}
