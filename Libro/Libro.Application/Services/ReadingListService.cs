using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Services
{
    public class ReadingListService : IReadingListService
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IPatronService _patronService;
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReadingListService> _logger;

        public ReadingListService(
            IReadingListRepository readingListRepository,
            IPatronService patronService,
            IMapper mapper,
            IBookService bookService,
            ILogger<ReadingListService> logger)
        {
            _readingListRepository = readingListRepository;
            _patronService = patronService;
            _mapper = mapper;
            _bookService = bookService;
            _logger = logger;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId)
        {
            try
            {
                // this code should be deleted from here
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while in ReadingListService retrieving the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }
        public async Task<ReadingList> GetReadingListByNameAsync(string listName, string patronId)
        {
            try
            {
                ReadingList readingList = await _readingListRepository.GetReadingListByNameAsync(listName, patronId);

                return readingList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while in ReadingListService retrieving the reading list with Name: {listName} for patron with ID: {patronId}.");
                throw;
            }
        }
        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId)
        {
            try
            {
                var patron = await _patronService.GetPatronAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }

                var readingLists = await _readingListRepository.GetReadingListsByPatronIdAsync(patronId);
                return readingLists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while retrieving reading lists for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<ReadingListDto> CreateReadingListAsync(ReadingListDto readingListDto, string patronId)
        {
            try
            {
                var patron = await _patronService.GetPatronAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
                }
                readingListDto.PatronId = patronId;
                // check if there is a reading list with the same name for that patron
                var readingList = _mapper.Map<ReadingList>(readingListDto);

                var createdList = await _readingListRepository.CreateReadingListAsync(readingList);
                return _mapper.Map<ReadingListDto>(createdList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while creating a reading list for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task RemoveReadingListAsync(int listId, string patronId)
        {
            try
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
                await _readingListRepository.RemoveReadingListAsync(readingList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while removing the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<IEnumerable<BookDto>> GetBooksByReadingListAsync(int listId, string patronId)
        {
            try
            {
                var patron = await _patronService.GetPatronAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }
                var readingList = await _readingListRepository.GetReadingListByIdAsync(listId, patronId);
                if (readingList == null)
                {
                    throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
                }

                var books = await _readingListRepository.GetBooksByReadingListAsync(readingList);
                return _mapper.Map<IEnumerable<BookDto>>(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while retrieving books for the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task<bool> AddBookToReadingListAsync(int listId, string patronId, string bookId)
        {
            try
            {
                var patron = await _patronService.GetPatronAsync(patronId);
                if (patron == null)
                {
                    throw new ResourceNotFoundException("Patron", "ID", patronId);
                }
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
                if (readingList.BookLists == null)
                {
                    readingList.BookLists = new List<BookList>(); // Initialize the BookLists property if null
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while adding a book with ISBN: {bookId} to the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }

        public async Task RemoveBookFromReadingListAsync(int listId, string patronId, string bookId)
        {
            try
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
                Book book = await _bookService.GetBookByISBNAsync(bookId);
                if (book == null || readingList.BookLists == null || !readingList.BookLists.Any(bl => bl.BookId == bookId))
                {
                    throw new ResourceNotFoundException("Book", "ISBN", bookId);
                }
                await _readingListRepository.RemoveBookFromReadingListAsync(readingList, bookId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred in ReadingListService while removing the book with ISBN: {bookId} from the reading list with ID: {listId} for patron with ID: {patronId}.");
                throw;
            }
        }
    }
}
