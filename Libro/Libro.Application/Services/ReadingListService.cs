using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class ReadingListService : IReadingListService
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IPatronService _patronService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public ReadingListService(IReadingListRepository readingListRepository, 
            IPatronService patronService, IMapper mapper, IBookService bookService)
        {
            _readingListRepository = readingListRepository;
            _patronService = patronService;
            _mapper = mapper;
            _bookService = bookService;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId)
        {
            var patron = await _patronService.GetPatronAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId);
            }
            ReadingList readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            if (readingList == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            return readingList;
        }

        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId)
        {
            var patron = await _patronService.GetPatronAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId);
            }
            var readingLists = await _readingListRepository.GetReadingListsByPatronIdAsync(patronId);
            return readingLists;
        }

        public async Task<ReadingListDto> CreateReadingListAsync(ReadingListDto readingListDto, string patronId)
        {
            var patron = _patronService.GetPatronAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            var readingList = _mapper.Map<ReadingList>(readingListDto);

            var createdList = await _readingListRepository.CreateReadingListAsync(readingList);
            return _mapper.Map<ReadingListDto>(createdList);
        }

        public async Task RemoveReadingListAsync(int listId, string patronId)
        {
            ReadingList list = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);

            await _readingListRepository.RemoveReadingListAsync(list);

        }
        public async Task<IEnumerable<BookDto>> GetBooksByReadingListAsync(int listId, string patronId)
        {
            var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            if (readingList == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            var books = await _readingListRepository.GetBooksByReadingListAsync(readingList, patronId);

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<bool> AddBookToReadingListAsync(int listId, string patronId, string bookId)
        {
            ReadingList readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            Book book = await _bookService.GetBookByISBNAsync(bookId);
            if (book == null)
            {
                throw new ResourceNotFoundException("Book", "ISBN", bookId);
            }
            if (readingList == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            bool bookExists = readingList.BookLists.Any(bl => bl.BookId == bookId);
            if (bookExists)
            {
                return false;
            }
            BookList bookList = new BookList() { ReadingList = readingList, Book = book };
            await _readingListRepository.AddBookToReadingListAsync(readingList, bookList);
            return true;
        }

        public async Task RemoveBookFromReadingListAsync(int listId, string patronId, string bookId)
        {
            ReadingList readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            Book book = await _bookService.GetBookByISBNAsync(bookId);
            if (readingList == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            if (book == null || !readingList.BookLists.Any(bl => bl.BookId == bookId))
            {
                throw new ResourceNotFoundException("Book", "ISBN", bookId);
            }
            await _readingListRepository.RemoveBookFromReadingListAsync(readingList, bookId);
        }
    }

}
