using AutoMapper;
using Libro.Domain.Common;
using Libro.Domain.Dtos;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
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

        public async Task<PaginatedResult<AuthorDto>> GetAllAuthorsAsync(int pageNumber, int pageSize)
        {
            var paginatedResult = await _authorRepository.GetAllAuthorsAsync(pageNumber, pageSize);

            var authorDtos = _mapper.Map<IEnumerable<AuthorDto>>(paginatedResult.Items);

            return new PaginatedResult<AuthorDto>(authorDtos, paginatedResult.TotalCount, pageNumber, pageSize);
        }

        public async Task<Author> GetAuthorByIdAsync(int authorId)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(authorId);
            return author; 
        }

        public async Task<Author> AddAuthorAsync(AuthorDto authorDto)
        {
            var author = _mapper.Map<Author>(authorDto);
            return await _authorRepository.AddAuthorAsync(author);
        }

        public async Task UpdateAuthorAsync(int authorId, AuthorDto authorDto)
        {
            Author author = await _authorRepository.GetAuthorByIdAsync(authorId);

            if (author == null)
                throw new ResourceNotFoundException("Author", "ID", authorId.ToString());

            author.Name = authorDto.Name;

            await _authorRepository.UpdateAuthorAsync(author);
        }

        public async Task DeleteAuthorAsync(int authorId)
        {
            Author author= await _authorRepository.GetAuthorByIdAsync(authorId);
            if (author == null)
            {
                throw new ResourceNotFoundException("Author", "ID", authorId.ToString());
            }
            await _authorRepository.DeleteAuthorAsync(author);
        }
    }
}
