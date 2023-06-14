using AutoMapper;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Interfaces.IRepositories;
using Libro.Domain.Interfaces.IServices;

namespace Libro.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = _authorRepository.GetAllAuthorsAsync();
            return _mapper.Map<IEnumerable<AuthorDto>>(authors);
        }

        public async Task<AuthorDto> GetAuthorByIdAsync(int authorId)
        {
            var author = _authorRepository.GetAuthorByIdAsync(authorId);
            return _mapper.Map<AuthorDto>(author);
        }

        public async Task<int> AddAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            return _authorRepository.AddAuthorAsync(author);
        }

        public async Task<bool> UpdateAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            return _authorRepository.UpdateAuthorAsync(author);
        }

        public async Task<bool> DeleteAuthorAsync(int authorId)
        {
            return _authorRepository.DeleteAuthorAsync(authorId);
        }
    }
}
