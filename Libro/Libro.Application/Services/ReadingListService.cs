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
        private readonly IPatronRepository _patronRepository;
        private readonly IMapper _mapper;

        public ReadingListService(IReadingListRepository readingListRepository, 
            IPatronRepository patronRepository, IMapper mapper)
        {
            _readingListRepository = readingListRepository;
            _patronRepository = patronRepository;
            _mapper = mapper;
        }

        public async Task<ReadingList> GetReadingListByIdAsync(int listId, string patronId)
        {
            var readingList = _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            if (readingList == null)
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());

            return readingList;
        }

        public async Task<IEnumerable<ReadingList>> GetReadingListsByPatronIdAsync(string patronId)
        {
            var readingLists = await _readingListRepository.GetReadingListsByPatronIdAsync(patronId);
            return readingLists;
        }

        public async Task<ReadingListDto> CreateReadingListAsync(ReadingListDto readingListDto, string patronId)
        {
            var patron = _patronRepository.GetPatronByIdAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            var readingList = _mapper.Map<ReadingList>(readingListDto);
            readingList.PatronId = patronId;

            var createdList = await _readingListRepository.CreateReadingListAsync(readingList);
            var createdListDto = _mapper.Map<ReadingListDto>(createdList);
            return createdListDto;
        }

        public async Task<bool> RemoveReadingListAsync(int listId, string patronId)
        {
            var patron = _patronRepository.GetPatronByIdAsync(patronId);
            if (patron == null)
            {
                throw new ResourceNotFoundException("Patron", "ID", patronId.ToString());
            }
            var list = _readingListRepository.GetReadingListByIdAsync(listId, patronId);
            if (list == null)
            {
                throw new ResourceNotFoundException("Reading List", "ID", listId.ToString());
            }
            return await _readingListRepository.RemoveReadingListAsync(listId, patronId);
        }
        public async Task<IEnumerable<BookDto>> GetBooksByReadingListAsync(int listId, string patronId)
        {
            var books = await _readingListRepository.GetBooksByReadingListAsync(listId, patronId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<bool> AddBookToReadingListAsync(int listId, string patronId, string bookId)
        {
            //var book = _bookRepository.GetBookById(bookId);
            //if (book == null)
            //{
            //    throw new ResourceNotFoundException("Book", "ISBN", bookId);
            //}
            var result = await _readingListRepository.AddBookToReadingListAsync(listId, patronId, bookId);
            return result;
        }

        public async Task<bool> RemoveBookFromReadingListAsync(int listId, string patronId, string bookId)
        {
            var result = await _readingListRepository.RemoveBookFromReadingListAsync(listId, patronId, bookId);
            return result;
        }
    }

}
